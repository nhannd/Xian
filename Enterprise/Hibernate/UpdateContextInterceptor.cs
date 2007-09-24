using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common.Specifications;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Implementation of NHibernate IInterceptor, used to record entity change-set for a transaction.
    /// </summary>
    internal class UpdateContextInterceptor : EmptyInterceptor
    {
        private Dictionary<object, EntityChange> _changeSet = new Dictionary<object, EntityChange>();
        private Queue<DomainObject> _pendingValidations = new Queue<DomainObject>();

        /// <summary>
        /// Returns the set of <see cref="EntityChange"/> capturing the changes made.
        /// </summary>
        public EntityChange[] EntityChangeSet
        {
            get { return new List<EntityChange>(_changeSet.Values).ToArray(); }
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
        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            RecordChange(entity, EntityChangeType.Delete);
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
        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            List<string> dirtyProperties = new List<string>();

            int propertyCount = propertyNames.Length;
            for (int i = 0; i < propertyCount; i++)
            {
                // check if the property is dirty
                // note: if the property is a collection, don't bother checking, just assume it may be dirty
                // the reason is that the cost of checking if the collection is dirty may be equal to or even greater than
                // the cost of re-validating it
                if (types[i] is NHibernate.Collection.PersistentCollection || !object.Equals(currentState[i], previousState[i]))
                {
                    dirtyProperties.Add(propertyNames[i]);
                }
            }

            Validate(entity, dirtyProperties);
            RecordChange(entity, EntityChangeType.Update);
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
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            // ignore the addition of auditing records
            if (NHibernateUtil.GetClass(entity).Equals(typeof(TransactionRecord)))
                return false;

            // don't validate the entity here, because further changes to it may be made before the flush
            // instead, put it in a queue to be validated at flush time
            _pendingValidations.Enqueue((DomainObject)entity);

            RecordChange(entity, EntityChangeType.Create);
            return false;

        }

        public override void PreFlush(ICollection entities)
        {
            while (_pendingValidations.Count > 0)
            {
                DomainObject obj = _pendingValidations.Dequeue();
                Validate(obj);
            }

            base.PreFlush(entities);
        }

        public override void PostFlush(ICollection entities)
        {
            base.PostFlush(entities);
        }

        #endregion

        private void RecordChange(object obj, EntityChangeType changeType)
        {
            // ignore changes to enum values for now
            // TODO: we should really not ignore these
            if (obj is EnumValue)
                return;

            Entity entity = (Entity)obj;

            EntityChange change = new EntityChange(new EntityRef(entity.GetClass(), entity.OID, entity.Version), changeType);
            if (_changeSet.ContainsKey(entity.OID))
            {
                // if this entity was already marked as changed, but the new change supercedes the previous
                // change, then overwrite with the new change
                if (change.Supercedes(_changeSet[entity.OID]))
                    _changeSet[entity.OID] = change;
            }
            else
            {
                // record this change in the change set
                _changeSet[entity.OID] = change;
            }
        }

        private void Validate(object obj, List<string> dirtyProperties)
        {
            ValidationRuleSet rules = Validation.GetInvariantRules((DomainObject)obj);

            TestResult result = rules.Test(obj, dirtyProperties);
            if (result.Fail)
            {
                string message = string.Format("Invalid {0}.", obj.GetType().Name);
                throw new EntityValidationException(message, result.Reasons);
            }
        }

        private void Validate(object obj)
        {
            Validate(obj, null);
        }
    }
}
