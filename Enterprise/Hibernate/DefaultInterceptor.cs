using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate
{
    public class DefaultInterceptor : IInterceptor
    {
        private List<EntityChange> _changes = new List<EntityChange>();

        /// <summary>
        /// Returns the set of entities that were changed during the session
        /// </summary>
        public EntityChange[] EntityChangeSet
        {
            get { return _changes.ToArray(); }
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
            _changes.Add(new EntityChange(NHibernateUtil.GetClass(entity), ent.OID, ent.Version, EntityChangeType.Delete));
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
            _changes.Add(new EntityChange(NHibernateUtil.GetClass(entity), ent.OID, ent.Version, EntityChangeType.Update));
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
            _changes.Add(new EntityChange(NHibernateUtil.GetClass(entity), ent.OID, ent.Version, EntityChangeType.Create));
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
