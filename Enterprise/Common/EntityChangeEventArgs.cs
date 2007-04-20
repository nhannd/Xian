using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
    public class EntityChangeEventArgs : EventArgs
    {
        private EntityChangeSet _changeSet;

        /// <summary>
        /// Constructor
        /// </summary>
        internal EntityChangeEventArgs(EntityChangeSet changeSet)
        {
            _changeSet = changeSet;
        }

        /// <summary>
        /// Change set
        /// </summary>
        public EntityChangeSet ChangeSet
        {
            get { return _changeSet; }
        }
    }
}
