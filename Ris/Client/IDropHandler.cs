#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface for handling drag and drop operations
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IDropHandler<TItem>
    {
        /// <summary>
        /// Asks the handler if it can accept the specified items.  This value is used to provide visual feedback
        /// to the user to indicate that a drop is possible.
        /// </summary>
        bool CanAcceptDrop(ICollection<TItem> items);

        /// <summary>
        /// Asks the handler to process the specified items, and returns true if the items were successfully processed.
        /// </summary>
        bool ProcessDrop(ICollection<TItem> items);
    }
}
