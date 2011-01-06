#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base implementation of <see cref="INotebox"/>.
    /// </summary>
    public abstract class Notebox : INotebox
    {
        /// <summary>
        /// Queries the notebox for its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        public abstract IList GetItems(INoteboxQueryContext nqc);

        /// <summary>
        /// Queries the notebox for a count of its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        public abstract int GetItemCount(INoteboxQueryContext nqc);

        /// <summary>
        /// Gets the invariant criteria for this class of notebox.
        /// </summary>
        /// <param name="wqc"></param>
        /// <returns></returns>
        public abstract NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc);
    }
}
