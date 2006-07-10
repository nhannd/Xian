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
        private Type _entityType;
        private EntityChangeType _changeType;

        public EntityChange(long entityOid, Type entityType, EntityChangeType changeType)
        {
            _entityOid = entityOid;
            _entityType = entityType;
            _changeType = changeType;
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
    }
}
