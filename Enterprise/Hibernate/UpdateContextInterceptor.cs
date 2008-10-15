#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using NHibernate.Type;
using NHibernate.Collection;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Implementation of NHibernate IInterceptor, used to record entity change-set for a transaction.
    /// </summary>
    internal class UpdateContextInterceptor : EmptyInterceptor
    {

        private readonly ChangeTracker _changeTracker = new ChangeTracker();
        private readonly Queue<DomainObject> _pendingValidations = new Queue<DomainObject>();

        /// <summary>
        /// Gets the set of <see cref="EntityChange"/> objects representing the changes made in this update context.
        /// </summary>
        public EntityChange[] EntityChangeSet
        {
            get { return _changeTracker.EntityChangeSet; }
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
            // build a list of property diffs
            // the current state is "null" since the entity was deleted
            PropertyDiff[] propertyDiffs = GetPropertyDiffs(propertyNames, types, null, state);
            RecordChange(entity, EntityChangeType.Delete, propertyDiffs);
        }

        /// <summary>
        /// Called when a dirty entity is flushed, which implies an update to the DB.
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
            // This method may be called more 
            // than once for a given entity during the lifetime of the update context, and the difference between the 
            // currentState and previousState parameters will reflect only the changes
            // to the entity that have occured since the last time this method was called.

            // build a list of property diffs
            PropertyDiff[] propertyDiffs = GetPropertyDiffs(propertyNames, types, currentState, previousState);


            // Build a list of dirty properties for validation
            // include collection properties even if not dirty, because we can't rely on the Equals for collections
            List<PropertyDiff> dirtyProperties = CollectionUtils.Select(propertyDiffs,
                delegate(PropertyDiff diff) { return diff.IsCollectionProperty || diff.IsChanged; });

            // validate the entity prior to flush, passing the list of dirty properties 
            // in order to optimize which rules are tested
            // rather than testing every validation rule we can selectively test only those rules
            // that may be affected by the modified state
            Validate(entity, CollectionUtils.Map<PropertyDiff, string>(dirtyProperties,
                delegate(PropertyDiff pc) { return pc.PropertyName; }));

            RecordChange(entity, EntityChangeType.Update, propertyDiffs);
            return false;
        }

        /// <summary>
        /// Called when a new entity is added to the persistence context via <see cref="PersistenceContext.Lock"/> with
        /// <see cref="DirtyState.New"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            // ignore the addition of log entries (any subclass of LogEntry)
            if (typeof(LogEntry).IsAssignableFrom(NHibernateUtil.GetClass(entity)))
                return false;

            // we could validate the entity here, but it seems rather counter-productive, given
            // that the entity is not actually being written to the DB yet and further changes
            // may be made to it by the application before it is written.  Therefore, we choose
            // not to validate the entity here, but instead put it in a queue to be validated at flush time
            _pendingValidations.Enqueue((DomainObject)entity);

            // build a list of property diffs
            // the previous state is "null" since the entity was just created
            PropertyDiff[] propertyDiffs = GetPropertyDiffs(propertyNames, types, state, null);
            RecordChange(entity, EntityChangeType.Create, propertyDiffs);
            return false;

        }

        /// <summary>
        /// Called prior to every flush.
        /// </summary>
        /// <param name="entities"></param>
        public override void PreFlush(ICollection entities)
        {
            // validate any transient entities that have not yet been validated
            // note that there is a possibility that NHibernate may do its own validation of new entities
            // prior to arriving here, in which case it will have already thrown an exception
            // this is unfortunate, because the exceptions that we generate are *much* more informative
            // and user-friendly, but there is no obvious solution to this as of NH1.0
            // TODO: NH1.2 added new methods to the Interceptor API - see if any of these will get around this problem
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

        private void RecordChange(object domainObject, EntityChangeType changeType, PropertyDiff[] propertyDiffs)
        {
            // ignore changes to enum values for now
            // TODO: should probably record changes to enum values as well
            if (domainObject is EnumValue)
                return;

            Entity entity = (Entity)domainObject;
            _changeTracker.RecordChange(entity, changeType, propertyDiffs);
        }

        private void Validate(object domainObject, List<string> dirtyProperties)
        {
            Validation.Validate((DomainObject)domainObject,
                delegate(ISpecification rule)
                {
                    // see if the rule needs to be checked (i.e if relevant properties are dirty)
                    return ShouldCheckRule(rule, (DomainObject)domainObject, dirtyProperties);
                });
        }

        private void Validate(object domainObject)
        {
            Validation.Validate((DomainObject)domainObject);
        }

        private bool ShouldCheckRule(ISpecification rule, DomainObject domainObject, List<string> dirtyProperties)
        {
            // if the rule is not bound to specific properties, then it should be checked
            if (!(rule is IPropertyBoundRule))
                return true;

            IPropertyBoundRule pbRule = rule as IPropertyBoundRule;

            // if the rule is bound to a property of an embedded value rather than the entity itself, then return true
            // (the rule won't actually be tested unless the property containing the embedded value is dirty)
            if(CollectionUtils.Contains(pbRule.Properties,
                delegate(PropertyInfo p) { return typeof(ValueObject).IsAssignableFrom(p.DeclaringType); }))
                return true;

            // otherwise, we assume the rule is bound to a property of the entity

            // if no properties are dirty, we don't need to check it
            if (dirtyProperties.Count == 0)
                return false;

            // if the rule is bound to any properties that are dirty, return true
            return CollectionUtils.Contains((rule as IPropertyBoundRule).Properties,
                        delegate(PropertyInfo prop) { return dirtyProperties.Contains(prop.Name); });
        }

        private PropertyDiff[] GetPropertyDiffs(string[] propertyNames, IType[] types, object[] currentState, object[] previousState)
        {
            PropertyDiff[] diffs = new PropertyDiff[propertyNames.Length];
            for(int i = 0; i < diffs.Length; i++)
            {
                object oldValue = previousState == null ? null : previousState[i];
                object newValue = currentState == null ? null : currentState[i];

				// need to handle collections specially
				if (types[i].IsCollectionType && ReferenceEquals(oldValue, newValue))
				{
					// the collection instance itself has not changed, but perhaps the content has?
					// if the oldValue is a persistent collection, then we can get a snapshot from
					// when the collection was initially loaded, and see if it is dirty
					if (oldValue is IPersistentCollection)
					{
						oldValue = GetCollectionSnapshot(oldValue as IPersistentCollection);
					}
				}

				diffs[i] = new PropertyDiff(propertyNames[i], types[i], oldValue, newValue);
			}
            return diffs;
        }

		private static object GetCollectionSnapshot(IPersistentCollection collection)
		{
			// the collection is not dirty, then the snapshot is the collection
			if (!collection.IsDirty)
			{
				return collection;
			}

			// it is dirty, so we need to get the snapshot
			ICollection snapshot = collection.CollectionSnapshot.Snapshot;
			
			// unfortunately, the snapshot is not always stored in the same data structure as the collection itself
			if(collection is ISet)
			{
				// "set"
				// we return an untyped set - this is a bit lazy, we could create a typed set with some extra effort, but do we need it?
				return new HybridSet(((IDictionary)snapshot).Values);
			}
			else if(collection is IList && snapshot is IDictionary)
			{
				// "idbag"
				// we return an untyped list - this is a bit lazy, we could create a typed list with some extra effort, but do we need it?
				return new ArrayList(((IDictionary) snapshot).Values);
			}
			else if(collection is IList && snapshot is IList)
			{
				// "list" or "bag"
				// in this case, the NH snapshot is the same type as the original collection
				return snapshot;
			}
			else if(collection is IDictionary)
			{
				// in this case, the NH snapshot is the same type as the original collection
				return snapshot;
			}
			else
			{
				// TODO: implement this for other types of collection
				throw new NotImplementedException("Snapshot is not implemented for this collection type.");
			}
		}
    }
}
