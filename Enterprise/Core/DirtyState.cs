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

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Used by <see cref="IUpdateContext.Lock"/> to indicate whether an entity should be considered clean or dirty.
    /// </summary>
    public enum DirtyState
    {
        /// <summary>
        /// Treat the entity as clean
        /// </summary>
        Clean,

        /// <summary>
        /// Treat the entity as dirty
        /// </summary>
        Dirty,

        /// <summary>
        /// Treat the entity as new (unsaved)
        /// </summary>
        New
    }
}
