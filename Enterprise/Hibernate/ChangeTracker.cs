using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Helper class used by <see cref="UpdateContextInterceptor"/> to track changes to entities during usage
    /// of an update context.
    /// </summary>
    internal class ChangeTracker
    {
        class ChangeRecord
        {
            private readonly Entity _entity;
            private readonly EntityChangeType _changeType;

            public ChangeRecord(Entity entity, EntityChangeType changeType)
            {
                _entity = entity;
                _changeType = changeType;
            }

            public Entity Entity { get { return _entity; } }

            public EntityChangeType ChangeType
            {
                get { return _changeType; }
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
            public bool Supercedes(ChangeRecord other)
            {
                if (!_entity.Equals(other.Entity))
                    throw new ArgumentException("Argument must represent a change to the same entity");

                return _changeType > other.ChangeType;
            }
        }

        // keeps a record of changes made to entities
        // a given entity instance will only appear once in this list
        // (a dictionary is not used because we cannot control the implemention of the entity's Equals/GetHashCode methods,
        // and we want to deal strictly with reference equality here)
        private readonly List<ChangeRecord> _changeRecords = new List<ChangeRecord>();

        /// <summary>
        /// Records that the specified change occured to the specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="changeType"></param>
        public void RecordChange(Entity entity, EntityChangeType changeType)
        {
            ChangeRecord thisChange = new ChangeRecord(entity, changeType);

            // check if this entity was already recorded in the list
            // note that reference equality is used because we do not know how the Equals method of the entity may be implemented
            // for example, it may define equality based on a property whose value has changed, which would break this logic
            int prevRecordIndex = _changeRecords.FindIndex(delegate(ChangeRecord r) { return ReferenceEquals(r.Entity, entity); });
            if (prevRecordIndex > -1)
            {
                // this entity was already marked as changed
                ChangeRecord previousChange = _changeRecords[prevRecordIndex];

                // if this change supercedes the previous change, then overwrite with the new change
                if (thisChange.Supercedes(previousChange))
                {
                    _changeRecords[prevRecordIndex] = thisChange;
                }
            }
            else
            {
                // record this change in the change set
                _changeRecords.Add(thisChange);
            }
        }

        /// <summary>
        /// Gets the set of <see cref="EntityChange"/> objects representing the cumulative changes made.
        /// </summary>
        public EntityChange[] EntityChangeSet
        {
            get
            {
                return CollectionUtils.Map<ChangeRecord, EntityChange>(_changeRecords,
                    delegate(ChangeRecord changeRecord)
                    {
                        return new EntityChange(changeRecord.Entity.GetRef(), changeRecord.ChangeType);
                    }).ToArray();
            }
        }
    }
}
