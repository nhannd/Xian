#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
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
        /// Constructor.
        /// </summary>
        internal ClosedEventArgs(CloseReason reason)
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
        /// Constructor.
        /// </summary>
		internal ClosedItemEventArgs(TItem item, CloseReason reason)
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
