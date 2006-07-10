using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// A simple implementation of ITransactionMonitor.  In reality, ITransactionMonitor should be implemented
    /// by a remote service running on the server, but for the time being there is a simple stub that
    /// exists as part of the client process.  The stub will transmit change set notifications within the client
    /// process only - hence multiple clients will not be aware of each others' changes.
    /// </summary>
    public class TransactionMonitor : ITransactionMonitor
    {
        private Queue<EntityChange> _pending;
        private Dictionary<Type, EventHandler<EntityChangeEventArgs>> _eventMap;
        private Session _session;

        internal TransactionMonitor(Session session)
        {
            _session = session;
            _pending = new Queue<EntityChange>();
            _eventMap = new Dictionary<Type, EventHandler<EntityChangeEventArgs>>();
        }


        #region ITransactionMonitor Members

        public void Subscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler)
        {
            if (_eventMap.ContainsKey(entityType))
            {
                _eventMap[entityType] += eventHandler;
            }
            else
            {
                _eventMap.Add(entityType, eventHandler);
            }
        }

        public void Unsubscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler)
        {
            if (_eventMap.ContainsKey(entityType))
            {
                _eventMap[entityType] -= eventHandler;
            }
        }

        public void Queue(EntityChange[] changeSet)
        {
            foreach (EntityChange change in changeSet)
            {
                _pending.Enqueue(change);
            }
        }

        public void PostPending()
        {
            while (_pending.Count > 0)
            {
                Notify(_pending.Dequeue());
            }
        }

        private void Notify(EntityChange change)
        {
            if (_eventMap.ContainsKey(change.EntityType))
            {
                EventsHelper.Fire(_eventMap[change.EntityType], _session, new EntityChangeEventArgs(change));
            }
        }

        #endregion
    }
}
