#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
using System.Reflection;

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
            Validation.Validate((DomainObject)obj,
                delegate(ISpecification rule)
                {
                    // see if the rule needs to be check (i.e if relevant properties are dirty)
                    return ShouldCheckRule(rule, (DomainObject)obj, dirtyProperties);
                });
        }

        private void Validate(object obj)
        {
            Validation.Validate((DomainObject)obj);
        }

        private bool ShouldCheckRule(ISpecification rule, DomainObject obj, List<string> dirtyProperties)
        {
            // if the rule is not bound to specific properties, then it should be checked
            if (!(rule is IPropertyBoundRule))
                return true;

            IPropertyBoundRule pbRule = rule as IPropertyBoundRule;

            // if the rule is bound to a property of an embedded value rather than the entity itself, then it should be checked
            if(CollectionUtils.Contains<PropertyInfo>(pbRule.Properties,
                delegate(PropertyInfo p) { return typeof(ValueObject).IsAssignableFrom(p.DeclaringType); }))
                return true;

            // otherwise, we assume the rule is bound to a property of the entity

            // if no properties are dirty, we don't need to check it
            if (dirtyProperties.Count == 0)
                return false;

            // check the rule if it is bound to any properties that are dirty
            return CollectionUtils.Contains<PropertyInfo>((rule as IPropertyBoundRule).Properties,
                        delegate(PropertyInfo prop) { return dirtyProperties.Contains(prop.Name); });
        }
    }
}
