#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Text;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Threading;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Message = ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference.Message;
using System.ServiceModel.Channels;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls;
using System.Net.NetworkInformation;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Views;
using System.Net;
using System.Net.Browser;
using System.Windows.Media;
using System.IO;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public class ChannelErrorEventArgs : EventArgs
    {
        public string ErrorName { get; set; }
        public string Details { get; set; }
    }

    public class ServerApplicationStopEventArgs : EventArgs
    {
        public ApplicationStoppedEvent ServerEvent { get; set; }
    }

    //TODO (CR May 2010): Generally, this class does more than it should and the name is not really valid anymore - it's not just a dispatcher, but more
    //of an "application manager", as it pretty much manages all aspects of a remote instance of an application.
    //Also, it is intermixing handling of the communication with display of messages.  Ideally, all messages would be displayed by a UI element based on
    //feedback from this class.
	public class ServerEventDispatcher : IDisposable
	{
        Thread _outboundThread;
        private delegate void DialogResultHandler();
        public delegate void ServerCallCompletedCallback(AsyncCompletedEventArgs ev);

        public event EventHandler<ServerApplicationStopEventArgs> ServerApplicationStopped;

        ///TODO (CR May 2010): we should be able to get rid of these "type handlers" entirely (I have a design change in mind).
        private readonly Dictionary<Type, EventHandler<ServerEventArgs>> _typeHandlers = new Dictionary<Type, EventHandler<ServerEventArgs>>();
        private readonly Dictionary<Guid, EventHandler<ServerEventArgs>> _sourceHandlers = new Dictionary<Guid, EventHandler<ServerEventArgs>>();
		private readonly object _sync = new object();
		private ApplicationServiceClient _proxy;
		private MessageQueue _queue;
        private ApplicationStartupParameters _appParameters;
	    private Dispatcher _dispatcher;
        private ApplicationContext _context;
        private StartApplicationRequest _startRequest;
        private bool _connectionOpened;

        private Binding _binding = null;

		private int _nextMessageId = 1;

        private Queue<MessageSet> _outboundQueue = new Queue<MessageSet>();
        private object _outboundQueueSync = new object();


        private Dictionary<int, EventSet> _incomingEventSets = new Dictionary<int, EventSet>();
        private int NextEventSetNumber = 1;

        private object _incomingEventSync = new object();
        private Dictionary<Guid, long> _timePrevTileUpdateEvent = new Dictionary<Guid, long>();
        private long renderLoopCount = 0;

        //TODO: this should not be here. Belong to the app instead?
        private ServerMessagePoller _poller;

		public bool Faulted { 
            get {
                if (_proxy != null && _proxy.State != CommunicationState.Faulted && _proxy.InnerChannel != null && _proxy.InnerChannel.State != CommunicationState.Faulted)
                    return false;

                return true;
            } 
        }

        public ApplicationServiceClient Proxy
        {
            get { return _proxy; }
        }

        public ServerEventDispatcher(ApplicationContext context)
        {
            _context = context;
            _dispatcher = Deployment.Current.Dispatcher;
        }

        public void Initialize(ApplicationStartupParameters appParameters)
		{
            try
            {
                _appParameters = appParameters;
                if (_queue == null)
                    _queue = new MessageQueue(DoSend);

                CompositionTarget.Rendering += CompositionTarget_Rendering;
				SetupChannel();

            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }
 
        private void OnError(Exception exception)
        {
            if (exception is CommunicationException)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    DisplayFaulted("Network Error", "Network connection has been lost.");
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(String.Format("{0}: {1}", exception.GetType(), exception.Message));
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(exception.StackTrace);


                    if (exception.InnerException != null)
                    {
                        sb.AppendLine(String.Format("{0}: {1}", exception.InnerException.GetType(), exception.InnerException.Message));
                        sb.AppendLine("Stack Trace:");
                        sb.AppendLine(exception.InnerException.StackTrace);
                    }

                    DisplayFaulted("Communication Error", sb.ToString());
                }
            }
            else if (exception is FaultException<SessionValidationFault>)
            {
                FaultException<SessionValidationFault> fault = exception as FaultException<SessionValidationFault>;
                DisplayFaulted("Session Error", fault.Detail.ErrorMessage);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(String.Format("{0}", exception.Message));
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    sb.AppendLine(String.Format("{0}: {1}", exception.InnerException.GetType(), exception.InnerException.Message));
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(exception.InnerException.StackTrace);
                }


                DisplayFaulted("Error", sb.ToString());
            }
        }

        private void SetupChannel()
        {
            Binding binding = GetChannelBinding();
            EndpointAddress remoteAddress = GetServerAddress();
            _proxy = new ApplicationServiceClient(binding, remoteAddress);

            _proxy.InnerChannel.Faulted += OnChannelFaulted;
            _proxy.InnerChannel.Opening += OnChannelOpening;
            _proxy.InnerChannel.Opened += Connection_Opened;

            _proxy.ProcessMessagesCompleted += new EventHandler<ProcessMessagesCompletedEventArgs>(MessageSent);
            //_proxy.GetPendingEventCompleted += new EventHandler<GetPendingEventCompletedEventArgs>(GetPendingEventCompleted);
            _proxy.StartApplicationCompleted += StartApplicationCompleted;
            _proxy.StopApplicationCompleted += StopApplicationCompleted;
            _proxy.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);
        }

        //TODO (CR May 2010): this should be part of a UI element.
        ChildWindow _stateDialog;
        private void OnChannelOpening(object sender, EventArgs e)
        {
            UIThread.Execute(() =>
            {    
                _stateDialog = PopupHelper.PopupMessage("Initialization", "Opening connection...");
            });
        }

        private void Connection_Opened(object sender, EventArgs e)
        {
            _connectionOpened = true;
            
            UIThread.Execute(() =>
            {
                if (_stateDialog != null)
                {
                    _stateDialog.Close();
                    _stateDialog = null;
                }
            });          
            
        }

        private void ReleaseChannel()
        {
            if (_proxy != null)
            {
                _proxy.InnerChannel.Opened -= Connection_Opened;
                _proxy.InnerChannel.Opening -= OnChannelOpening;
                _proxy.ProcessMessagesCompleted -= MessageSent;
                _proxy.StartApplicationCompleted -= StartApplicationCompleted;
                _proxy.StopApplicationCompleted -= StopApplicationCompleted;
                _proxy.InnerChannel.Faulted -= OnChannelFaulted;
                _proxy.CloseAsync();
                _proxy = null;
                _connectionOpened = false;
            }
        }
  
		private EndpointAddress GetServerAddress()
        {
          
            switch(ApplicationStartupParameters.Current.Mode)
            {
                case ApplicationServiceMode.BasicHttp:
                                var uri = "../Services/ApplicationService.svc/basicHttp";
                                var address = new EndpointAddress(new Uri(uri, UriKind.RelativeOrAbsolute)); 
                                return address;
            }

            // This should never occur
            throw new Exception();
        }

        private Binding GetChannelBinding()
        {

            switch(ApplicationStartupParameters.Current.Mode)
            {
                    case ApplicationServiceMode.BasicHttp:
                                var binaryMessageEncoding = new BinaryMessageEncodingBindingElement();

                                if (HtmlPage.Document.DocumentUri.Scheme.Equals(Uri.UriSchemeHttp))
                                {
                                    HttpTransportBindingElement http = new HttpTransportBindingElement() { MaxReceivedMessageSize = int.MaxValue, MaxBufferSize = int.MaxValue, TransferMode = TransferMode.Buffered };
                                    _binding = new CustomBinding(binaryMessageEncoding, http);
                                    return _binding;
                                }
                                else
                                {
                                    HttpsTransportBindingElement https = new HttpsTransportBindingElement() { MaxReceivedMessageSize = int.MaxValue, MaxBufferSize = int.MaxValue, TransferMode = TransferMode.Buffered };
                                    _binding = new CustomBinding(binaryMessageEncoding, https);
                                    return _binding;
                                }
                
            }
                
            return null;
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            //TODO (CR May 2010) How to deal with string resources?
            DisplayFaulted("Connection Error", _connectionOpened? "Connection to the server has been lost.": String.Format("Could not establish connection to {0}", _proxy.InnerChannel.RemoteAddress.Uri));
            UIThread.Execute(() =>
            {
                if (_stateDialog != null)
                {
                    _stateDialog.Close();
                    _stateDialog = null;
                }
            });
        }

	    private void StartApplicationCompleted(object sender, StartApplicationCompletedEventArgs e)
	    {
            if (e.Error != null )
            {
                OnError(e.Error);
                return;
            }

            ApplicationContext.Current.ID = e.Result.AppIdentifier;

            ThrottleSettings.Default.PropertyChanged += new PropertyChangedEventHandler(ThrottleSettings_PropertyChanged);

            //TODO: put the key in some assembly that can be shared with the server-side code
            _proxy.SetPropertyAsync(new SetPropertyRequest{ ApplicationId = ApplicationContext.Current.ID , 
                        Key = "DynamicImageQualityEnabled",
                        Value= ThrottleSettings.Default.EnableDynamicImageQuality.ToString() });

            StartPolling();

            _outboundThread = new Thread((ignore)=>{ ProcessOutboundQueue(); } );

            _outboundThread.Start();
	    }

        private void StartPolling()
        {
            _poller = new ServerMessagePoller(_proxy);
            _poller.MessageReceived += (sender, ev) => { OnServerEventReceived(ev.EventSet); };
            _poller.Start();
        }

        private void ThrottleSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("EnableDynamicImageQuality"))
            {
                _proxy.SetPropertyAsync(new SetPropertyRequest
                {
                    ApplicationId = ApplicationContext.Current.ID,
                    Key = "DynamicImageQualityEnabled",
                    Value = ThrottleSettings.Default.EnableDynamicImageQuality.ToString()
                });

            }
        }

        private void StopApplicationCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ServerCallCompletedCallback callback = e.UserState as ServerCallCompletedCallback;
            if (callback!=null)
            {
                callback(e);
            }
            else
            {
                //TODO (CR May 2010) This shouldn't be in the else, error could happen when a callback is available.
                if (e.Error != null && _connectionOpened)
                {
                    DisplayFaulted("Error", e.Error.Message);
                }
            }
        }

        private void MessageSent(object sender, ProcessMessagesCompletedEventArgs e)
	    {
            // TODO: REVIEW THIS
            // Per MSDN:
            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
            ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount; 
            MessageSet msgs = e.UserState as MessageSet;

            PerformanceMonitor.CurrentInstance.DecrementMouseWheelMsgCount(msgs.Messages.Count((i) => i is MouseWheelMessage));

            if (e.Error != null)
            {
                OnError(e.Error);
                return;
            }

            if (e.Result!=null)
            {
                if (e.Result.EventSet!=null && e.Result.EventSet.Events!=null)
                {
                    bool isMoveMoveMsg =  msgs.Messages.Any((i) => i is MouseMoveMessage);
                    if (isMoveMoveMsg)
                    {
                    
                        long dt = Environment.TickCount - msgs.Tick;
                        var p = PerformanceMonitor.CurrentInstance;

                        bool tileUpdateEventReturned = e.Result.EventSet != null && e.Result.EventSet.Events.Any((i) => i is TileUpdatedEvent);

                        p.LogMouseMoveRTTWithResponse(msgs.Number, dt);
                    }                 

                    if (e.Result.EventSet != null)
                    {
                        OnServerEventReceived(e.Result.EventSet);
                    }
                }
                
            }
            

	    }
        
        //TODO (CR May 2010): see my comments at the top RE: separation of responsibilities.
        private void DisplayFaulted(string error, string details)
        {
            try
            {
                ErrorHandler.HandleCriticalError(details);
            }
            finally
            {
            	// In some case, this is not necessary because the connection is already faulted.
            	// But let's do it anyway.
                Disconnect(details);
            }
        }

        private void OnApplicationNotFoundEventReceived(ApplicationNotFoundEvent applicationNotFoundEvent)
        {
            //TODO:  Think about if this is the right solution
            // NOTE: _startRequest contains the sessionid which is probably expired by this time.
            if (ApplicationContext.Current.ID.Equals(applicationNotFoundEvent.ApplicationId))
            {
                DisplayFaulted("Synchronization Lost",
                            String.Format("The application has not been found on the server: {0}", applicationNotFoundEvent.ApplicationId));
            }
        }

        private void OnApplicationStoppedEventReceived(ApplicationStoppedEvent applicationStoppedEvent)
        {
            try
            {
                if (ServerApplicationStopped != null)
                {
                    ServerApplicationStopped(this, new ServerApplicationStopEventArgs { ServerEvent = applicationStoppedEvent });
                }
                else
                {
                    ErrorHandler.HandleCriticalError(applicationStoppedEvent.Message);
                }
            }
            finally
            {
                Disconnect("Application has stopped");
            }
        }

        public void ProcessOutboundQueue()
        {
            while(true)
            {
                lock (_outboundQueueSync)
                {
                    if (_outboundQueue.Count == 0)
                    {
                        Monitor.Wait(_outboundQueueSync);
                    }
                }


                if (_outboundQueue.Count == 0)
                {
                    continue;
                }

                if (_outboundQueue.Count > 0)
                {
                    MessageSet msgset = _outboundQueue.Dequeue();
                    msgset.Tick = Environment.TickCount;
                    msgset.Timestamp = DateTime.Now; 
                    
                    _proxy.ProcessMessagesAsync(msgset, msgset);

                    ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;
                }
            }
        }
        
	    private void DoSend(List<Message> msgs)
	    {
			if (Faulted)
				return;

            //TODO (CR May 2010): we just checked Faulted and returned, so this probably will never happen.
            if (_proxy.State == CommunicationState.Faulted
                || _proxy.InnerChannel.State == CommunicationState.Faulted)
            {
                DisplayFaulted("Error", "Unexpected error");
                return;
            }

            
                MessageSet msgset = new MessageSet();
                msgset.Messages = new System.Collections.ObjectModel.ObservableCollection<Message>();
                msgset.ApplicationId = _context.ID;
                msgset.Number = _nextMessageId++;

                foreach (Message msg in msgs)
                {
                    if (msg != null)
                        msgset.Messages.Add(msg);
                }
                try
                    {

                        // TODO: REVIEW THIS
                        // Per MSDN:
                        // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                        // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                        msgset.Tick = Environment.TickCount;
                        msgset.Timestamp = DateTime.Now;

                        if (ThrottleSettings.Default.SimulateNetworkTrafficOrder)
                        {
                            lock (_outboundQueueSync)
                            {
                                _outboundQueue.Enqueue(msgset);
                                Monitor.Pulse(_outboundQueueSync);
                            }
                        }
                        else
                        {
                            Logger.Write(String.Format("<== {0}: MSG # {1}\n", Environment.TickCount, msgset.Number));
                            _proxy.ProcessMessagesAsync(msgset, msgset);

                            // TODO: REVIEW THIS
                            // Per MSDN:
                            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                            ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;
                        }
                        
                    }
                    catch (Exception e)
                    {
                        // happens on timeout of connection
                        ErrorHandler.HandleException(e);
                        if (_proxy.State == CommunicationState.Faulted)
                            DisplayFaulted("Channel Error", e.Message);
                    }
	    }

		/// <summary>
		/// Register an event handler for a specific type of event from the server
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="handler"></param>
        public void RegisterEventHandler(Type eventType, EventHandler<ServerEventArgs> handler)
		{
            lock (_sync)
			{
                if (_typeHandlers.ContainsKey(eventType))
                {
                    var existingHandlers = _typeHandlers[eventType];
                    handler += existingHandlers;
                    // strange but must replace what's in the list or we will lose it.. 
                    _typeHandlers[eventType] = handler;
                }
                else
				    _typeHandlers.Add(eventType, handler);
			}
		}

		//TODO: this doesn't seem to work even though we kind of need it to
		public void UnregisterEventHandler(Type eventType, EventHandler<ServerEventArgs> handler)
		{
			lock (_sync)
			{
				if (_typeHandlers.ContainsKey(eventType))
				{
					EventHandler<ServerEventArgs> h = _typeHandlers[eventType];
					h -= handler;
                    //TODO (CR May 2010) Assign h back to _typeHanders[eventType], like above?
                    //  _typeHandlers[eventType] = h;

                }
			}
		}

		/// <summary>
		/// Register an event handler from a specific sender/resource from the server
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="handler"></param>
        public void RegisterEventHandler(Guid sender, EventHandler<ServerEventArgs> handler)
		{
			lock (_sync)
			{
				_sourceHandlers.Add(sender, handler);
			    Logger.Write(String.Format("Register event handler for {0}\n", sender));
			}
		}


        /// <summary>
        /// Release an event handler from a specific sender/resource from the server
        /// </summary>
        /// <param name="sender"></param>
		public void UnregisterEventHandler(Guid sender)
        {
            lock (_sync)
            {
                _sourceHandlers.Remove(sender);
                Logger.Write(String.Format("Release event handler for {0}\n", sender));
            }
        }


		private void ProcessEventSet(EventSet eventSet)
        {
            if (eventSet == null)
                return;

            if (eventSet.Events == null)
                return;

#if DEBUG
            if (eventSet.Events!=null)
            {
                int updateEvents =  eventSet.Events.Count((i)=> i is TileUpdatedEvent);
                if (updateEvents>1)
                {
                    //UIThread.Execute(()=> System.Windows.MessageBox.Show(String.Format("Multiple tile update events in message set #{0}", eventSet.Number)));
                }
            }
#endif

            
            foreach (Event @event in eventSet.Events)
			{
                var ev = @event;
                
                EventHandler<ServerEventArgs> handler;

                try
				{
                    if (ev is ApplicationNotFoundEvent)
                    {
                        OnApplicationNotFoundEventReceived(ev as ApplicationNotFoundEvent);
                    }
                    else if (ev is ApplicationStoppedEvent)
                    {
                        OnApplicationStoppedEventReceived(ev as ApplicationStoppedEvent);
                    }
                    else if (_sourceHandlers.TryGetValue(ev.SenderId, out handler))
                    {
                        handler.Invoke(ev.SenderId, new ServerEventArgs { ServerEvent = ev });
                    }
                    else if (_typeHandlers.TryGetValue(ev.GetType(), out handler))
                    {
                        handler.Invoke(ev.SenderId, new ServerEventArgs { ServerEvent = ev });
                    }
                    else
                    {
                        string msg;
                        if (ev is PropertyChangedEvent)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(String.Format("EventID:{0}", ev.Identifier));
                            sb.AppendLine(String.Format("Source: {0} [ID={1}]", ev.Sender, ev.SenderId));
                            sb.AppendLine(String.Format("Property: {0}", (ev as PropertyChangedEvent).PropertyName));
                            sb.AppendLine(String.Format("Value: {0}", (ev as PropertyChangedEvent).Value));
                            msg = sb.ToString();
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(String.Format("EventID:{0}", ev.Identifier));
                            sb.AppendLine(String.Format("Source: {0} [ID={1}]", ev.Sender, ev.SenderId));
                            msg = sb.ToString();                                
                        }

                        PopupHelper.PopupMessage("Error Handler Not Found", msg);
                    }
				}
				catch (Exception ex)
				{
					if (_proxy.State == CommunicationState.Faulted)
                        DisplayFaulted("Channel Error", ex.Message);
                }
			}
		}

        // TODO: should return bool instead
	    public void DispatchMessage(Message message)
        {
            try
            {
                if (!Faulted)
                {
                    

                    if (message is MouseMoveMessage)
                    {
                        PerformanceMonitor.CurrentInstance.IncrementSendLag(1);
                    }
                    else if (message is MouseWheelMessage)
                    {
                        PerformanceMonitor.CurrentInstance.IncrementMouseWheelMsgCount(1);
                    }

                    ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;
                    _queue.Enqueue(message);
                } 
                
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
	    }

		public void StartApplication(StartApplicationRequest request)
		{
            if (_proxy != null)
            {
                _startRequest = request;
                _proxy.StartApplicationAsync(request);
            }
		}

        internal void PublishPerformance(PerformanceData data)
        {
            _proxy.ReportPerformanceAsync(data);
        }

		public void StopApplication(Guid identifier)
		{
			_proxy.StopApplicationAsync(new StopApplicationRequest { ApplicationId = identifier });
		}

        public void Disconnect(string reason)
        {
            if (_proxy != null)
            {
                //TODO (CR May 2010): we don't sync the proxy anywhere else.
                lock (_sync)
                {
                    if (_poller != null)
                    {
                        _poller.Dispose();
                        _poller = null;
                    }

                    if (_proxy != null)
                    {
                        ReleaseChannel();
                        _proxy = null;
                    }
                }                
            }
        }

        public void Dispose()
        {
            // TODO: This method is called on Application Exit. 
            // But SL does not support calling web service on this event
            if (_proxy != null)
            {
                Disconnect("Disposing");
            }

            if (_queue != null)
            {
                _queue.Dispose();
                _queue = null;
            }

            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void OnServerEventReceived(EventSet eventSet)
        {
            Logger.Write(String.Format("==> {0}: IN MSG # {1}\n", Environment.TickCount, eventSet.Number));
            
            if (ThrottleSettings.Default.LagDetectionStrategy == LagDetectionStrategy.WhenMouseMoveIsProcessed)
                PerformanceMonitor.CurrentInstance.DecrementSendLag(eventSet.Events.Count((i) => i is MouseMoveProcessedEvent));

            int tileUpdateEvCount = eventSet.Events.Count((i) => i is TileUpdatedEvent);
            if (tileUpdateEvCount > 0)
            {
                //if (ThrottleSettings.Default.LagDetectionStrategy == LagDetectionStrategy.WhenTileUpdateReturn)
                //    PerformanceMonitor.CurrentInstance.DecrementSendLag(eventSet.Events.Count((i) => i is TileUpdatedEvent));

                PerformanceMonitor.CurrentInstance.RenderingLag += tileUpdateEvCount;
                if (tileUpdateEvCount>1)
                    Logger.Write(String.Format("########## {0} tile update events received ###########\n",tileUpdateEvCount));
            }


            lock (_incomingEventSync)
            {
                _incomingEventSets[eventSet.Number] = eventSet;
                Monitor.Pulse(_incomingEventSync);
            }
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            ProcessIncomingQueue(null);
        }


        
        internal void ProcessIncomingQueue(object ignore)
        {
            EventSet current;
            lock (_incomingEventSync)
            {
                //TODO: will renderLoopCount overflow?
                renderLoopCount++;
               
                if (!_incomingEventSets.TryGetValue(NextEventSetNumber, out current))
                {
                    return;
                }
                    
                bool hasTileUpdateEvent = current.Events.Any((i) => i is TileUpdatedEvent);
                TileUpdatedEvent tileUpdateEv = hasTileUpdateEvent ? current.Events.First((i) => i is TileUpdatedEvent) as TileUpdatedEvent : null;
                    
                if (hasTileUpdateEvent)
                {
                    long prevTileUpdate=0;
                    if (_timePrevTileUpdateEvent.TryGetValue(tileUpdateEv.SenderId, out prevTileUpdate))
                    {
                        // For some reason, image on the screen is not updated if it is changed too fast
                        // My guess is it takes another iteration to refresh the UI.
                        //
                        // NOTE: From MSDN http://msdn.microsoft.com/en-us/library/system.windows.media.compositiontarget.rendering.aspx
                        // This event handler gets called once per frame. Each time that Windows Presentation Foundation (WPF) marshals the persisted rendering data in the visual tree across to the composition tree, 
                        // your event handler is called. In addition, if changes to the visual tree force updates to the composition tree, your event handler is also called. 
                        // Note that your event handler is called after layout has been computed. 
                        // However, you can modify layout in your event handler, which means that layout will be computed once more before rendering.
                        //
                        if (ThrottleSettings.Default.EnableFPSCap)
                            if (renderLoopCount - prevTileUpdate < 3)
                                return;
                    }                   
                }

                _incomingEventSets.Remove(NextEventSetNumber);
                NextEventSetNumber = current.Number + 1;
                var ev = current;

                ProcessEventSet(ev); 

                if (hasTileUpdateEvent)
                {
                    _timePrevTileUpdateEvent[tileUpdateEv.SenderId] = renderLoopCount;
                }
            }
		}
    }

	class MessageQueue : IDisposable
    {
        public delegate void SendDelegate(List<Message> msgs);

		private Thread _thread;
		private volatile bool _stop;
		private readonly object _syncLock = new object();
        private readonly Queue<Message> _queue = new Queue<Message>();

        public MessageQueue(SendDelegate del)
        {
            _thread = new Thread(ProcessQueue);
			_thread.Name = String.Format("Outbound Message Queue [{0}]", _thread.ManagedThreadId);
			_thread.Start(del);
        }

        public void Enqueue(Message msg)
        {
			lock (_syncLock)
			{
				_queue.Enqueue(msg);
				Monitor.Pulse(_syncLock);
			}
        }

        public int Count
        {
			get
			{
				lock (_syncLock)
				{
					return _queue.Count;
				}
			}
        }

        private void ProcessQueue(object del)
        {
            SendDelegate sendDelegate = del as SendDelegate;
            while (!_stop)
            {
				const int maxMessages = 2;
                List<Message> msgs = new List<Message>();
				lock (_syncLock)
				{
					if (_queue.Count == 0)
						Monitor.Wait(_syncLock);

					while(_queue.Count > 0 && msgs.Count < maxMessages)
						msgs.Add(_queue.Dequeue());

					if (msgs.Count == 0)
						continue;

				}

                // send is called outside the lock block to avoid deadlock when polling duplex http _binding is used
                // it happens when the server for some reason decides to wait for the client to finish processing the "app started" message,
                // and the client attempts to send the "client rect size" msg to the server when it processes he "app started" message.
                //
                // Basic http _binding appears to have the same problem too
                try
                {
                    sendDelegate(msgs);
                }
                catch (Exception)
                {
                    //??
                }
            }
        }

        public void Dispose()
        {
			if (_stop)
				return;

			_stop = true;
			lock (_syncLock) { Monitor.Pulse(_syncLock); }

            //TODO (CR May 2010) Add 30 second timeout on _thread.Join()
			if (_thread.IsAlive)
				_thread.Join();
        }
    }

}


namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference
{

    public partial class MessageSet
    {
        public int Tick;
    }
}
