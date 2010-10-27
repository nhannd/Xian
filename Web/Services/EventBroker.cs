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
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
	//TODO (CR May 2010): queue, not broker (name?)
    internal class EventBroker : IDisposable
    {
		private readonly Application _application;
        private readonly IApplicationServiceCallback _callback;

		private volatile bool _suspended;

		private readonly Thread _thread;
		private readonly object _syncLock = new object();
		private readonly Queue<Event> _queue;
		private int _nextEventSetNumber = 1;
		private bool _stop;

		private int _prevMsgTick;

		public EventBroker(Application application, IApplicationServiceCallback client)
        {
		    Platform.CheckForNullReference(application, "application");
            Platform.CheckForNullReference(client, "client");
            _application = application;
            _callback = client;

			_queue = new Queue<Event>();
			_thread = new Thread(ProcessQueue);
			_thread.Name = String.Format("Web Event Broker:{0}", _thread.ManagedThreadId);
			_thread.Start();
		}

    	public bool Suspended
    	{
    		get { return _suspended; }
			set
			{
				_suspended = value;
				if (value)
					return;

				lock(_syncLock)
				{
					Monitor.Pulse(_syncLock);
				}
			}
    	}

		public void Send(Event @event)
		{
		    Platform.CheckForNullReference(@event, "event");
			lock (_syncLock)
			{
				_queue.Enqueue(@event);
				if (!_suspended || !@event.AllowSendInBatch)
					Monitor.Pulse(_syncLock);
			}
		}

		private void DoSend(List<Event> events)
        {
            LogEvents(events);

			try
            {
                _callback.EventNotification(new EventSet
                                            	{
                                            		Events = events.ToArray(), 
													ApplicationId = _application.Identifier,
													Number = _nextEventSetNumber++
                                            	});
            }
            catch (ObjectDisposedException)
            {
                // Just eat this error
            }
			catch(CommunicationException)
			{
				// Just eat this error
			}
			catch (Exception e)
            {
				_application.Stop(e);
            }
        }

        private void LogEvents(IList<Event> events)
        {
        	//TODO (CR May 2010): ConsoleHelper?
#if DEBUG
            int elapsed = Environment.TickCount - _prevMsgTick;
            ConsoleColor normal = Console.ForegroundColor;
			if (events.Count == 1)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
			}
            else
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("*** OPTIMIZED: Sending {0} events in a single message ******", events.Count);
			}

			foreach (Event e in events)
                Console.WriteLine("<-- [+{0,4}ms] : {1}]", elapsed, e);
            
			Console.ForegroundColor = normal;
            _prevMsgTick = Environment.TickCount;
#endif
        }

        private void ProcessQueue()
        {
			Application.Current = _application;

			const int maxBatchSize = 20;

            while (true)
            {
				Event singleEvent = null;
				List<Event> events = new List<Event>();

				lock (_syncLock)
            	{
					//IMPORTANT: we can't stop until we've at least tried to send all the messages
					//because the ApplicationStopped event is fired right before the app disposes this object
					if (_stop && _queue.Count == 0)
						break;
					
					if (_queue.Count == 0)
						Monitor.Wait(_syncLock);

					if (_stop && _queue.Count == 0)
						break;

					while (_queue.Count > 0 && events.Count < maxBatchSize)
					{
						Event @event = _queue.Dequeue();
						if (!@event.AllowSendInBatch)
						{
							singleEvent = @event;
							break;
						}

						events.Add(@event);
            		}
                }

                if (events.Count > 0)
                    DoSend(events);

                if (singleEvent != null)
                    DoSend(new List<Event> { singleEvent });
			}

			Application.Current = null;
        }

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				lock(_syncLock)
				{
					_stop = true;
					Monitor.Pulse(_syncLock);
				}

				if (Thread.CurrentThread.Equals(_thread))
				{
					Platform.Log(LogLevel.Debug, "Disposing EventBroker from it's own thread.");
					return;					
				}
				
				if (_thread.IsAlive)
					_thread.Join(TimeSpan.FromSeconds(30));
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Unexpected error disposing EventBroker.");
			}
		}

		#endregion
	}   
}