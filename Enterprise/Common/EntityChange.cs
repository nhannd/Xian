using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Used by class <see cref="EntityChange"/> to record the type of change made to an entity.
    /// </summary>
    public enum EntityChangeType
    {
        Update = 0,
        Create = 1,
        Delete = 2
    }
    
    /// <summary>
    /// Acts as a memento of a change made to an entity.
    /// </summary>
    public class EntityChange
    {
        private object _entityOid;
        private int _version;
        private Type _entityClass;
        private EntityChangeType _changeType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityOid"></param>
        /// <param name="version"></param>
        /// <param name="changeType"></param>
        public EntityChange(Type entityType, object entityOid, int version, EntityChangeType changeType)
        {
            _entityOid = entityOid;
            _entityClass = entityType;
            _changeType = changeType;
            _version = version;
        }

        /// <summary>
        /// The type of change
        /// </summary>
        public EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// The class of the entity
        /// </summary>
        public Type EntityClass
        {
            get { return _entityClass; }
        }

        /// <summary>
        /// The entity OID
        /// </summary>
        public object EntityOID
        {
            get { return _entityOid; }
        }

        /// <summary>
        /// The entity version
        /// </summary>
        public int Version
        {
            get { return _version; }
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
            if (!_entityClass.Equals(other._entityClass) || !_entityOid.Equals(other._entityOid))
                throw new ArgumentException("Argument must represent a change to the same entity");

            return _version > other._version || (_version == other._version && _changeType > other._changeType);
        }
    }
}
