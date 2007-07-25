using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Used by class <see cref="EntityChange"/> to record the type of change made to an entity.
    /// </summary>
    [Serializable]
    public enum EntityChangeType
    {
        Update = 0,
        Create = 1,
        Delete = 2
    }
    
    /// <summary>
    /// Acts as a memento of a change made to an entity.
    /// </summary>
    [DataContract]
    public class EntityChange
    {
        private EntityRef _entityRef;
        private EntityChangeType _changeType;

        /// <summary>
        /// Constructor
        /// </summary>
        public EntityChange(EntityRef entityRef, EntityChangeType changeType)
        {
            _entityRef = entityRef;
            _changeType = changeType;
        }

        /// <summary>
        /// The type of change
        /// </summary>
        [DataMember]
        public EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// Reference to the entity that changed
        /// </summary>
        [DataMember]
        public EntityRef EntityRef
        {
            get { return _entityRef; }
        }

        /// <summary>
        /// Checks whether this change supercedes the specified other change.  This change supercedes other iff
        /// the version of this change is greater than the version of the other change, the version of this change
        /// is the same as the version of the other change and the <see cref="ChangeType"/>
        /// of this change supercedes the <see cref="ChangeType"/> of the other.  Note that
        /// <see cref="EntityChangeType.Create"/> supercedes <see cref="EntityChangeType.Update"/>, and 
        /// <see cref="EntityChangeType.Delete"/> supercedes both (e.g. a Create followed by an update is fundamentally a Create, and
        /// a Create or Update followed by a Delete is fundamentally a Delete).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Supercedes(EntityChange other)
        {
            if (!_entityRef.ClassName.Equals(other._entityRef.ClassName) || !_entityRef.OID.Equals(other._entityRef.OID))
                throw new ArgumentException("Argument must represent a change to the same entity");

            return _entityRef.Version > other._entityRef.Version || (_entityRef.Version == other._entityRef.Version && _changeType > other._changeType);
        }
    }
}
