using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Used by <see cref="ChangeTracker"/> to record changes to entities.
    /// </summary>
    internal class ChangeRecord
    {
        private readonly Entity _entity;
        private readonly EntityChangeType _changeType;
        private readonly PropertyDiff[] _propertyDiffs;

        public ChangeRecord(Entity entity, EntityChangeType changeType, PropertyDiff[] propertyDiffs)
        {
            _entity = entity;
            _changeType = changeType;
            _propertyDiffs = propertyDiffs;
        }

        public Entity Entity
        {
            get { return _entity; }
        }

        public EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        public PropertyDiff[] PropertyDiffs
        {
            get { return _propertyDiffs; }
        }

        public EntityChange AsEntityChange()
        {
            List<PropertyChange> propertyChanges = CollectionUtils.Map<PropertyDiff, PropertyChange>(
                GetRelevantPropertyDiffs(), delegate(PropertyDiff diff) { return diff.AsPropertyChange(); });

            return new EntityChange(_entity.GetRef(), _changeType, propertyChanges.ToArray());
        }

        /// <summary>
        /// Returns a new change record that represents the total of this change
        /// and the previous change.
        /// </summary>
        /// <param name="previousChange"></param>
        /// <returns></returns>
        public ChangeRecord Compound(ChangeRecord previousChange)
        {
            // assume the propertyDiffs array in both objects is aligned
            PropertyDiff[] resultDiffs = new PropertyDiff[_propertyDiffs.Length];
            for (int i = 0; i < _propertyDiffs.Length; i++)
            {
                resultDiffs[i] = _propertyDiffs[i].Compound(previousChange.PropertyDiffs[i]);
            }

            // return a new change record that represents the accumulation of both changes
            // the resultant ChangeType depends on whether this change Supercedes previousChange, or vice versa
            return new ChangeRecord(_entity, Supercedes(previousChange) ? _changeType : previousChange._changeType, resultDiffs);
        }

        /// <summary>
        /// Checks whether this change supercedes the specified other change.  This change supercedes other iff
        /// the <see cref="ChangeType"/> of this change is greater than the <see cref="ChangeType"/> of the other.
        /// </summary>
        /// <remarks>
        /// The <see cref="EntityChangeType.Create"/> value supercedes <see cref="EntityChangeType.Update"/>, and 
        /// <see cref="EntityChangeType.Delete"/> supercedes both.  In other words, a Create followed by an update
        /// is fundamentally a Create, and a Create or Update followed by a Delete is fundamentally a Delete.
        /// </remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool Supercedes(ChangeRecord other)
        {
            return _changeType > other.ChangeType;
        }

        /// <summary>
        /// Gets only the property diffs that are relevant to this change type.
        /// </summary>
        /// <returns></returns>
        private List<PropertyDiff> GetRelevantPropertyDiffs()
        {
            switch (_changeType)
            {
                // for creates, include all properties except "Version"
                case EntityChangeType.Create:
                    return CollectionUtils.Select(_propertyDiffs,
                            delegate(PropertyDiff diff)
                            {
                                return diff.PropertyName != "Version";
                            });

                // for updates, include only the properties that have actually changed
                case EntityChangeType.Update:
                    return CollectionUtils.Select(_propertyDiffs,
                            delegate(PropertyDiff diff)
                            {
                                return diff.PropertyName != "Version" && diff.IsChanged;
                            });

                // include no properties    
                case EntityChangeType.Delete:
                default:
                    return new List<PropertyDiff>();

            }
        }
    }
}
