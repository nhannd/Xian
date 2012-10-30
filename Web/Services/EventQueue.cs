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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
    public enum EventBatchMethod
    {
        PerType,
        PerTarget
    }

    public class EventQueue : IEventDeliveryStrategy, IDisposable
    {
        private class Item
        {
            public Item(Event @event, Action<Event> eventReleased)
            {
                Event = @event;
                EventReleased = eventReleased;
            }

            public readonly Event Event;
            public readonly Action<Event> EventReleased;
        }

        private readonly EventBatchMethod _eventBatchMethod;
        private Guid _applicationId;
        private readonly object _syncLock = new object();
		private readonly Queue<Item> _queue;
		private int _nextEventSetNumber = 1;
        
		public EventQueue() : this(EventBatchMethod.PerTarget)
        {
		}

        public EventQueue(EventBatchMethod eventBatchMethod)
        {
            _eventBatchMethod = eventBatchMethod;
            _queue = new Queue<Item>();
        }

        #region IEventDeliveryStrategy Members

        Guid IEventDeliveryStrategy.ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }

        void IEventDeliveryStrategy.Deliver(Event @event, Action<Event> eventReleased)
		{
            Enqueue(@event, eventReleased);
		}

        #endregion

        protected virtual void Enqueue(Event @event, Action<Event> eventReleased)
        {
            Platform.CheckForNullReference(@event, "event");
            lock (_syncLock)
            {
                _queue.Enqueue(new Item(@event, eventReleased));
            }
        }
        
        /// <summary>
        /// Check for pending events
        /// </summary>
        /// <param name="wait">Milliseconds</param>
        /// <returns></returns>
        public EventSet GetPendingEvents(int wait)
        {
            bool debugging = Platform.IsLogLevelEnabled(LogLevel.Debug);
            lock (_syncLock)
            {
                if (debugging)
                    Platform.Log(LogLevel.Debug, "Polling Request received.. {0} in the queue", _queue.Count);

                if (_queue.Count == 0)
                {
                    if (wait == 0)
                    {
                        return null;
                    }

                    if (debugging)
                        Platform.Log(LogLevel.Debug, "Nothing in the queue... waiting for {0} ms", wait);

                    //TODO: What happens if the browser is closed while waiting?
                    if (!Monitor.Wait(_syncLock, wait))
                    {
                        if (debugging)
                            Platform.Log(LogLevel.Debug, "Waiting ends. Still nothing in the queue");

                        return null;
                    }
                }


                const int maxBatchSize = 20;
                var markedEvents = new Dictionary<Guid, List<Event>>();

                var items = new List<Item>();
                try
                {
                    while (_queue.Count > 0 && items.Count < maxBatchSize)
                    {
                        var item = _queue.Peek();

                        // In theory this should never happen
                        if (item == null)
                        {
                            break;
                        }

                        // TODO (Phoenix5): Restore. 
                        //if (!@event.AllowSendInBatch)
                        //{
                        //    Event theEvent = @event;

                        //    if (_eventBatchMethod == EventBatchMethod.PerType)
                        //    {
                        //        // Only include the event if there's no other event of the same type to be sent.
                        //        if (events.Exists(i => i.GetType() == theEvent.GetType()))
                        //            break;
                        //    }
                        //    else if (_eventBatchMethod == EventBatchMethod.PerTarget)
                        //    {
                        //        // Note: We can send messages of the same type but targetted to different
                        //        // entities in the same response instead of separated ones to improve performance. 
                        //        // One example in the web station is during sync stacking
                        //        List<Event> speciallyMarkedEvents;
                        //        if (markedEvents.TryGetValue(@event.SenderId, out speciallyMarkedEvents))
                        //        {
                        //            if (speciallyMarkedEvents.Find(i => i.GetType() == @theEvent.GetType()) != null)
                        //                break;

                        //            markedEvents[@event.SenderId].Add(@event);
                        //        }
                        //        else
                        //        {
                        //            markedEvents.Add(@event.SenderId, new List<Event>(new[] {@event}));
                        //        }
                        //    }
                        //}

                        item = _queue.Dequeue();
                        items.Add(item);
                    }
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                }

                if (debugging)
                {
                    Platform.Log(LogLevel.Debug, "Sending Polling Response with {0} events", items.Count);

                    foreach(var item in items)
                    {
                        Platform.Log(LogLevel.Debug, "Event Sent: {0} [{1}]", item.Event.GetType().Name, item.Event.ToString());
                    }
                }

                if (items.Count > 0)
                {
                    ThreadPool.QueueUserWorkItem(o => ItemsReleased(items));

                    return new EventSet
                    {
                        Events = items.Select(i => i.Event).ToArray(),
                        ApplicationId = _applicationId,
                        Number = _nextEventSetNumber++,
                        HasMorePending = _queue.Count > 0,
                    };
                }
            }
            return null;
        }

        private static void ItemsReleased(IEnumerable<Item> items)
        {
            foreach (var item in items.Where(item => item.EventReleased != null))
                item.EventReleased(item.Event);
        }

        #region IDisposable Members

		public void Dispose()
		{
			try
			{
				lock(_syncLock)
				{
                    // Release any calls to GetPendingEvent() that may be in a Monitor.Wait()
                	Monitor.Pulse(_syncLock);

                    // CR 3-22-2011, is this necessary and enough time?
                    if (_queue.Count > 0)
                        Monitor.Wait(_syncLock, 5);
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