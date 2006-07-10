using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public class EntityChangeEventArgs : EventArgs
    {
        private EntityChange _change;

        public EntityChangeEventArgs(EntityChange change)
        {
            _change = change;
        }

        public EntityChange Change
        {
            get { return _change; }
        }
    }
}
