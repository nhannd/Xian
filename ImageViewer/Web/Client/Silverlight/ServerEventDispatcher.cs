#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public class ChannelErrorEventArgs : EventArgs
    {
        public string ErrorName { get; set; }
        public string Details { get; set; }
        public bool IsReloadAllowed { get; set; }
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
        private delegate void DialogResultHandler();
        public delegate void ServerCallCompletedCallback(AsyncCompletedEventArgs ev);

        public event EventHandler<ChannelErrorEventArgs> ChannelError;
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
		private int _pendingSend;
		private IncomingEventQueue _incomingEventQueue;
        private StartApplicationRequest _startRequest;
        private bool _connectionOpened;

		public bool Faulted { 
            get {
                if (_proxy != null && _proxy.State != CommunicationState.Faulted && _proxy.InnerChannel != null && _proxy.InnerChannel.State != CommunicationState.Faulted)
                    return false;

                return true;
            } 
        }


        public ServerEventDispatcher(ApplicationContext context)
        {
            _context = context;
            _dispatcher = Deployment.Current.Dispatcher;
        }

		public int PendingSend
		{
			get { return Interlocked.Add(ref _pendingSend, 0); }
		}

        public void Initialize(ApplicationStartupParameters appParameters)
		{
            try
            {
                _appParameters = appParameters;
                if (_queue == null)
                    _queue = new MessageQueue(DoSend);

				if (_incomingEventQueue == null)
					_incomingEventQueue = new IncomingEventQueue(ProcessEventSet);

				SetupChannel();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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
            _proxy.EventNotificationReceived += ServerEventReceived;
            _proxy.ProcessMessagesCompleted += MessageSent;
            _proxy.StartApplicationCompleted += StartApplicationCompleted;
            _proxy.StopApplicationCompleted += StopApplicationCompleted;
            _proxy.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);
        }

        //TODO (CR May 2010): this should be part of a UI element.
        DialogControl _stateDialog;

        private void OnChannelOpening(object sender, EventArgs e)
        {
            UIThread.Execute(() =>
            {
                _stateDialog = DialogControl.PopupMessage("Initialization", "Opening connection...");
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
                _proxy.EventNotificationReceived -= ServerEventReceived;
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
            string uri = string.Format("{0}://{1}:{2}/ApplicationServices",
                                        _appParameters.ServerSettings.LANMode? Uri.UriSchemeNetTcp: Uri.UriSchemeHttp,
                                        HtmlPage.Document.DocumentUri.Host,
                                        _appParameters.ServerSettings.Port);
            EndpointAddress address = new EndpointAddress(uri); 
            return address;
        }

        private Binding GetChannelBinding()
        {
            Binding binding = null;

            BinaryMessageEncodingBindingElement binaryMessageEncoding = new BinaryMessageEncodingBindingElement();
            TcpTransportBindingElement tcpTransport = new TcpTransportBindingElement() { MaxReceivedMessageSize = int.MaxValue, MaxBufferSize = int.MaxValue };
            tcpTransport.ConnectionPoolSettings.IdleTimeout = _appParameters.ServerSettings.InactivityTimeout;

            // Net tcp in SL4 does not provide transport level security :(
            // TransportSecurityBindingElement sec = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            // binding = new CustomBinding(binaryMessageEncoding, sec, tcpTransport);

            binding = new CustomBinding(binaryMessageEncoding, tcpTransport);
            
            return binding;
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            //TODO (CR May 2010) How to deal with string resources?
            DisplayFaulted( _connectionOpened? "Connection to the server has been lost.": String.Format("Could not establish connection to {0}", _proxy.InnerChannel.RemoteAddress.Uri));
            UIThread.Execute(() =>
            {
                if (_stateDialog != null)
                {
                    _stateDialog.Close();
                    _stateDialog = null;
                }
            });
        }

	    private void StartApplicationCompleted(object sender, AsyncCompletedEventArgs e)
	    {            
            if (e.Error != null && _connectionOpened)
            {
                if (e.Error is FaultException<SessionValidationFault>)
                {
                    FaultException<SessionValidationFault> fault = e.Error as FaultException<SessionValidationFault>;
                    DisplayFaulted("Error", fault.Detail.ErrorMessage, false);
                }
                else
                {

                    DisplayFaulted(e.Error.Message);
                }
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
                    DisplayFaulted( e.Error.Message);
                }
            }
        }

	    private void MessageSent(object sender, AsyncCompletedEventArgs e)
	    {
            MessageSet msgs = e.UserState as MessageSet;
            Interlocked.Add(ref _pendingSend, - msgs.Messages.Count);

            if (e.Error != null && _connectionOpened)
            {
                if (e.Error.GetType().Equals(typeof(CommunicationException)))
                {
                    if (!NetworkInterface.GetIsNetworkAvailable())
                        DisplayFaulted("Network connection has been lost.");
                    else
                        DisplayFaulted(e.Error.Message);
                }
                else
                    DisplayFaulted(e.Error.Message);
            }
	    }

        private void DisplayFaulted(string errorMessage)
        {
            DisplayFaulted("Error", errorMessage, true);
        }

		private void DisplayFaulted(string errorMessage, bool allowReload)
        {
            DisplayFaulted("Error", errorMessage, allowReload);
        }

        //TODO (CR May 2010): see my comments at the top RE: separation of responsibilities.
        private void DisplayFaulted(string error, string details, bool allowReload)
        {
            try
            {
                if (ChannelError != null)
                    ChannelError(_proxy, new ChannelErrorEventArgs { ErrorName = error, Details = details, IsReloadAllowed = allowReload });
                else
                {
                    Logger.Error(details);
                }
            }
            finally
            {
            	// In some case, this is not necessary because the connection is already faulted.
            	// But let's do it anyway.
                Disconnect();
            }
        }

        private void OnApplicationNotFoundEventReceived(ApplicationNotFoundEvent applicationNotFoundEvent)
        {
            //TODO:  Think about if this is the right solution
            // NOTE: _startRequest contains the sessionid which is probably expired by this time.
            if (ApplicationContext.Current.ID.Equals(applicationNotFoundEvent.ApplicationId))
            {
                DisplayFaulted("Synchronization Lost",
                            String.Format("The application has not been found on the server: {0}", applicationNotFoundEvent.ApplicationId), true);
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
                    Logger.Error(applicationStoppedEvent.Message);
                }
            }
            finally
            {
                Disconnect();
            }
        }

		private int _nextMessageId = 1;
	    private void DoSend(List<Message> msgs)
	    {
			if (Faulted)
				return;

            //TODO (CR May 2010): we just checked Faulted and returned, so this probably will never happen.
            if (_proxy.State == CommunicationState.Faulted
                || _proxy.InnerChannel.State == CommunicationState.Faulted)
            {
                DisplayFaulted( string.Empty);
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

                _proxy.ProcessMessagesAsync(msgset, msgset);
            }
            catch (Exception e)
            {
                // happens on timeout of connection
                Logger.Error(e);
                if (_proxy.State == CommunicationState.Faulted)
                    DisplayFaulted( e.Message);
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

		public void ServerEventReceived(object sender, EventNotificationReceivedEventArgs e)
		{
			if (e.Error != null)
			{
				if (_proxy.State == CommunicationState.Faulted)
					DisplayFaulted(e.Error.Message);
				return;
			}

			if (e.events == null || e.events.Events == null || e.events.Events.Count == 0)
			{
				Logger.Write("Recieve an empty event from the server");
			}
			else if (e.Cancelled)
			{
				StringBuilder sb = new StringBuilder();
				foreach (Event @event in e.events.Events)
				{
					if (sb.Length != 0)
						sb.AppendFormat(", ");

					sb.AppendFormat("{0}", @event.Identifier);
				}
				Logger.Write("Received server event: Asynchronous events were cancelled: {0}");

                //TODO (CR May 2010) Fault the channel?
			}
			else
			{
				_incomingEventQueue.ProcessEventSet(e.events);
			}
		}

		private void ProcessEventSet(EventSet eventSet)
		{
			foreach (Event ev in eventSet.Events)
			{
				EventHandler<ServerEventArgs> handler;
				Logger.Write("Received {0} {1} from {2}\n", ev.GetType().Name, ev.Identifier, ev.SenderId);

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

                        DialogControl.Show("Event Handler Not Found", msg, "Dismiss");
                    }
				}
				catch (Exception ex)
				{
					if (_proxy.State == CommunicationState.Faulted)
                        DisplayFaulted( ex.Message);
                }
			}
		}

	    public void DispatchMessage(Message message)
        {
            try
            {
                if (!Faulted)
                {
                    Interlocked.Increment(ref _pendingSend);
                    _queue.Enqueue(message);
                    //Logger.Write(String.Format("Pending messages: {0}\n", PendingSend));
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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

        public void Disconnect()
        {
            if (_proxy != null)
            {
                //TODO (CR May 2010): we don't sync the proxy anywhere else.
                lock (_sync)
                {
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
            if (_proxy != null)
            {
                Disconnect();
            }

            if (_queue != null)
            {
                _queue.Dispose();
                _queue = null;
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

					sendDelegate(msgs);
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
