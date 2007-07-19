using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides data for Closed events.
    /// </summary>
    public class ClosedEventArgs : EventArgs
    {
        private CloseReason _reason;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reason"></param>
        public ClosedEventArgs(CloseReason reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Gets the reason that the object was closed.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }
    }

    /// <summary>
    /// Provides data for ItemClosed events.
    /// </summary>
    /// <typeparam name="TItem">Type of the item that was closed.</typeparam>
    public class ClosedItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private CloseReason _reason;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reason"></param>
        public ClosedItemEventArgs(TItem item, CloseReason reason)
            :base(item)
        {
            _reason = reason;
        }

        /// <summary>
        /// Gets the reason that the item was closed.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }
    }
}
