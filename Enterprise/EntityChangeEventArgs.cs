using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public class EntityChangeEventArgs : EventArgs
    {
        private EntityRefBase _entityRef;
        private EntityChangeType _changeType;

        public EntityChangeEventArgs(EntityRefBase entityRef, EntityChangeType changeType)
        {
            _entityRef = entityRef;
            _changeType = changeType;
        }

        public EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        public EntityRefBase EntityRef
        {
            get { return _entityRef; }
        }
    }
}
