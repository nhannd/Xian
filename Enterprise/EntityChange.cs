using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Used by class <see cref="EntityChange"/> to record the type of change made to an entity.
    /// </summary>
    public enum EntityChangeType
    {
        Create,
        Update,
        Delete
    }
    
    /// <summary>
    /// Acts as a memento of a change made to an entity.
    /// </summary>
    public class EntityChange
    {
        private long _entityOid;
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
        public EntityChange(Type entityType, long entityOid, int version, EntityChangeType changeType)
        {
            _entityOid = entityOid;
            _entityClass = entityType;
            _changeType = changeType;
            _version = version;
        }

        /// <summary>
        /// The type of change
        /// </summary>
        internal EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// The class of the entity
        /// </summary>
        internal Type EntityClass
        {
            get { return _entityClass; }
        }

        /// <summary>
        /// The entity OID
        /// </summary>
        internal long EntityOID
        {
            get { return _entityOid; }
        }

        /// <summary>
        /// The entity version
        /// </summary>
        internal int Version
        {
            get { return _version; }
        }
    }
}
