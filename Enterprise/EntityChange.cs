using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public enum EntityChangeType
    {
        Create,
        Update,
        Delete
    }
    
    public class EntityChange
    {
        private long _entityOid;
        private int _version;
        private Type _entityType;
        private EntityChangeType _changeType;

        public EntityChange(Type entityType, long entityOid, int version, EntityChangeType changeType)
        {
            _entityOid = entityOid;
            _entityType = entityType;
            _changeType = changeType;
            _version = version;
        }

        public EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public long EntityOID
        {
            get { return _entityOid; }
        }

        public int Version
        {
            get { return _version; }
        }
    }
}
