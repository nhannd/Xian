using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Implementation of NHibernate IInterceptor, used to record entity change-set for a transaction.
    /// </summary>
    internal class DefaultInterceptor : IInterceptor
    {
        class ChangeRecord
        {
            private Entity _entity;
            private EntityChangeType _changeType;

            public ChangeRecord(object entity, EntityChangeType changeType)
            {
                _entity = (Entity)entity;
                _changeType = changeType;
            }

            public Entity Entity
            {
                get { return _entity; }
            }

            public EntityChangeType ChangeType
            {
                get { return _changeType; }
            }
        }


        private List<ChangeRecord> _changeRecords = new List<ChangeRecord>();
        private EntityChange[] _changeSet;

        /// <summary>
        /// Returns the set of <see cref="EntityChange"/> reflecting the changes that were made during the session
        /// </summary>
        public EntityChange[] EntityChangeSet
        {
            get
            {
                if (_changeSet == null)
                    throw new InvalidOperationException(SR.ExceptionAttemptToAccessChangeSetBeforeFlush);

                return _changeSet;
            }
        }

        public void ClearChangeSet()
        {
            _changeRecords.Clear();
            _changeSet = null;
        }

        #region IInterceptor Members

        /// <summary>
        /// Called when an entity is deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        public void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            _changeRecords.Add(new ChangeRecord(entity, EntityChangeType.Delete));
        }

        /// <summary>
        /// Called when a dirty entity is flushed, which implies an update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="currentState"></param>
        /// <param name="previousState"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            _changeRecords.Add(new ChangeRecord(entity, EntityChangeType.Update));
            return false;
        }

        /// <summary>
        /// Called when a new entity is saved for the first time
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            // HACK: ignore the addition of auditing records
            if (NHibernateUtil.GetClass(entity).Equals(typeof(TransactionRecord)))
                return false;

            _changeRecords.Add(new ChangeRecord(entity, EntityChangeType.Create));
            return false;

        }

        public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return null;
        }

        public object Instantiate(Type type, object id)
        {
            return null;
        }

        public object IsUnsaved(object entity)
        {
            return null;
        }

        public void PostFlush(System.Collections.ICollection entities)
        {
            // from the individual change records, construct a change set that contains each entity only once
            Dictionary<object, EntityChange> changes = new Dictionary<object, EntityChange>();
            foreach (ChangeRecord cr in _changeRecords)
            {
                EntityChange change = new EntityChange(new EntityRef(cr.Entity.GetClass(), cr.Entity.OID, cr.Entity.Version), cr.ChangeType);

                if (changes.ContainsKey(cr.Entity.OID))
                {
                    // this entity is already in the change set, so see if this change supercedes the previous one
                    EntityChange previousChange = changes[cr.Entity.OID];
                    if (change.Supercedes(previousChange))
                        changes[cr.Entity.OID] = change;
                }
                else
                {
                    // this entity is not yet in the change set, so add it
                    changes[cr.Entity.OID] = change;
                }
            }

            // convert to array
            _changeSet = (new List<EntityChange>(changes.Values)).ToArray();
        }

        public void PreFlush(System.Collections.ICollection entities)
        {
        }

        public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return false;

        }

        #endregion
    }
}
