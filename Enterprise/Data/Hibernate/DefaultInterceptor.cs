using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Data.Hibernate
{
    /// <summary>
    /// Implementation of NHibernate IInterceptor, used to record entity change-set for a transaction.
    /// </summary>
    public class DefaultInterceptor : IInterceptor
    {
        private Dictionary<object, EntityChange> _changes = new Dictionary<object, EntityChange>();

        /// <summary>
        /// Returns the set of entities that were changed during the session
        /// </summary>
        public ICollection<EntityChange> EntityChangeSet
        {
            get { return _changes.Values; }
        }

        public void ClearChangeSet()
        {
            _changes.Clear();
        }

        private void AddChange(EntityChange change)
        {
            if (_changes.ContainsKey(change.EntityOID))
            {
                // check if the previous change to this entity has been succeeded by another change with a higher ChangeType
                EntityChange prevChange = _changes[change.EntityOID];
                if (change.Supercedes(prevChange))
                    _changes[change.EntityOID] = change;
            }
            else
            {
                _changes.Add(change.EntityOID, change);
            }
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
            Entity ent = (Entity)entity;
            AddChange(new EntityChange(NHibernateUtil.GetClass(entity), ent.OID, ent.Version, EntityChangeType.Delete));
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
            Entity ent = (Entity)entity;
            AddChange(new EntityChange(NHibernateUtil.GetClass(entity), ent.OID, ent.Version, EntityChangeType.Update));
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
            Entity ent = (Entity)entity;
            AddChange(new EntityChange(NHibernateUtil.GetClass(entity), ent.OID, ent.Version, EntityChangeType.Create));
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
