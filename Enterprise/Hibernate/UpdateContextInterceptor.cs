using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Implementation of NHibernate IInterceptor, used to record entity change-set for a transaction.
    /// </summary>
    internal class UpdateContextInterceptor : EmptyInterceptor
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

        class ValidationRecord
        {
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
            Validate(entity);
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

            Validate(entity);
            RecordChange(entity, EntityChangeType.Create);
            return false;

        }

        public override void PostFlush(System.Collections.ICollection entities)
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

        #endregion

        private void RecordChange(object obj, EntityChangeType changeType)
        {
            // ignore changes to enum values for now
            // TODO: we should really not ignore these
            if (obj is EnumValue)
                return;

            _changeRecords.Add(new ChangeRecord(obj, changeType));
        }

        private void Validate(object obj)
        {
            ValidationRuleSet rules = Validation.GetInvariantRules(obj);
            TestResult result = rules.Test(obj);
            if (result.Fail)
                throw new EntityValidationException(result.Reasons);
        }
    }
}
