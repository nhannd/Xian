using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// A simple implementation of ITransactionMonitor.  In reality, ITransactionMonitor should be implemented
    /// by a remote service running on the server, but for the time being there is a simple stub that
    /// exists as part of the client process.  The stub will transmit change set notifications within the client
    /// process only - hence multiple clients will not be aware of each others' changes.
    /// </summary>
    public class TransactionNotifier : ITransactionNotifier
    {
        private Dictionary<Type, EventHandler<EntityChangeEventArgs>> _eventMap;
        private Session _session;

        internal TransactionNotifier(Session session)
        {
            _session = session;
            _eventMap = new Dictionary<Type, EventHandler<EntityChangeEventArgs>>();
        }


        #region ITransactionNotifier Members

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
            // in a real implementation, the changeSet would be forwarded to some server
            // but for now we just post the changes as local events within this process
            foreach (EntityChange change in changeSet)
            {
                Notify(change);
            }
        }

        #endregion

        private void Notify(EntityChange change)
        {
            if (_eventMap.ContainsKey(change.EntityClass))
            {
                EntityRefBase entityRef = EntityRefFactory.CreateReference(change.EntityClass, change.EntityOID, change.Version);

                EventsHelper.Fire(_eventMap[change.EntityClass], _session, new EntityChangeEventArgs(entityRef, change.ChangeType));
            }
        }
    }
}
