#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Specifies possible synchronization modes for a <see cref="IUpdateContext"/>
    /// </summary>
    public enum UpdateContextSyncMode
    {
        /// <summary>
        /// The context may write changes to the persistent store as necessary
        /// </summary>
        Flush,

        /// <summary>
        /// The context must hold all changes in memory
        /// </summary>
        Hold
    }
}
