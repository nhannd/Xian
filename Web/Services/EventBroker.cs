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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
	//TODO (CR May 2010): queue, not broker (name?)
    internal class EventBroker : IDisposable
    {
		private readonly Application _application;

        private readonly object _getPendingLock = new object();
		private readonly object _syncLock = new object();
		private readonly Queue<Event> _queue;
		private int _nextEventSetNumber = 1;

		public EventBroker(Application application)
        {
		    Platform.CheckForNullReference(application, "application");
            _application = application;

            _queue = new Queue<Event>();

		}

		public void Send(Event @event)
		{
		    Platform.CheckForNullReference(@event, "event");
			lock (_syncLock)
			{
				_queue.Enqueue(@event);
				Monitor.Pulse(_syncLock);
			}
		}

        internal EventSet GetPendingOutboundEvent(int wait)
        {
            // TODO: What happens if the client for some reason stops polling
            // The queue will get too big and cause out of memory?

            if (wait == 0 && _queue.Count == 0)
                return null;

            // only has effect if we decide to use multiple threads.
            lock (_getPendingLock)
            {
                const int maxBatchSize = 20;
                Dictionary<Guid, List<Event>> markedEvents = new Dictionary<Guid, List<Event>>();
            
                List<Event> events = new List<Event>();

                //IMPORTANT: we can't stop until we've at least tried to send all the messages
                //because the ApplicationStopped event is fired right before the app disposes this object
                int queueCount;
                lock (_syncLock)
                {
                    queueCount = _queue.Count;
                }

                if (queueCount == 0)
                {
                    if (wait == 0)
                        return null;

                    lock (_syncLock)
                    {
                        // wait until there's something in the queue
                        // this duration is like the long polling in silverlight duplex binding

                        //TODO: What happens if the browser is closed while waiting?
                        if (!Monitor.Wait(_syncLock, wait))
                            return null;

                        queueCount = _queue.Count;
                        if (queueCount == 0)
                            return null;
                    }
                }

                lock (_syncLock)
                {
                    while (_queue.Count > 0 && events.Count < maxBatchSize)
                    {
                        Event @event = _queue.Peek();

                        // In theory this should never happen
                        if (@event == null)
                            break;

                        if (!@event.AllowSendInBatch)
                        {
                            List<Event> speciallyMarkedEvents;
                            Event theEvent = @event;
                                    
                            switch(_application.BatchMode)
                            {
                                case MessageBatchMode.PerType:
                                    // Only include the event if there's no other event of the same type to be sent.
                                    if (events.Exists(i => i.GetType().Equals(theEvent.GetType())))
                                        break;

                                    break;

                                case MessageBatchMode.PerTarget:
                                    // Note: We can send messages of the same type but targetted to different
                                    // entities in the same response instead of separated ones to improve performance. 
                                    // One example in the web station is during sync stacking
                                    if (markedEvents.TryGetValue(@event.SenderId, out speciallyMarkedEvents))
                                    {
                                        if (speciallyMarkedEvents.Find((i) => i.GetType() == @theEvent.GetType()) != null)
                                            break;

                                        markedEvents[@event.SenderId].Add(@event);
                                    }
                                    else
                                    {
                                        markedEvents.Add(@event.SenderId, new List<Event>(new[] { @event }));
                                    } 
                                    break;
                            }
                            
                        }

                        @event = _queue.Dequeue();
                        events.Add(@event);
                    }
                }
                if (events.Count > 0)
                {
                    return new EventSet
                               {
                                   Events = events.ToArray(),
                                   ApplicationId = _application.Identifier,
                                   Number = _nextEventSetNumber++,
                                   HasMorePending = _queue.Count>0,
                               };

                }

                return null;
            }
        }

        #region IDisposable Members

		public void Dispose()
		{
			try
			{
				lock(_syncLock)
				{
                    // TODO: what to do with the rest of the messages in the queue?
                    // Should we send back all pending messages on the current poll? 
                    // What to do if the client has stopped polling (how can the server tells) ?
					Monitor.Pulse(_syncLock);
				}

			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Unexpected error disposing EventBroker.");
			}
		}

		#endregion
	}   
}