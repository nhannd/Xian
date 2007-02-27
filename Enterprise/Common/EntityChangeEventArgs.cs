using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Used by <see cref="ITransactionNotifier"/> to publish notifications about changes to entities.
    /// </summary>
    public class EntityChangeEventArgs : EventArgs
    {
        private EntityRef _entityRef;
        private EntityChangeType _changeType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityRef"></param>
        /// <param name="changeType"></param>
        internal EntityChangeEventArgs(EntityRef entityRef, EntityChangeType changeType)
        {
            _entityRef = entityRef;
            _changeType = changeType;
        }

        /// <summary>
        /// The type of change
        /// </summary>
        public EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// Reference to the changed entity
        /// </summary>
        public EntityRef EntityRef
        {
            get { return _entityRef; }
        }
    }
}
