#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Shreds
{
    /// <summary>
    /// Defines the set of operations that are possible on a Shred
    /// </summary>
    /// <remarks>See also <see cref="ShredExtensionPoint"/></remarks>
    public interface IShred
    {
        /// <summary>
        /// Shred should initialize all required resources and data structures and begin 
        /// exeuction of its mainline code
        /// </summary>
        void Start();
        /// <summary>
        /// Shred should stop, and release all held resources.
        /// </summary>
        void Stop();
        /// <summary>
        /// Shred should return a human-readable, friendly name that will be used in
        /// display lists and other human-readable user-interfaces.
        /// </summary>
        /// <returns></returns>
        string GetDisplayName();
        /// <summary>
        /// Shred should return a lengthier description of what it was created for and 
        /// what it was created to do.
        /// </summary>
        /// <returns></returns>
        string GetDescription();
    }
}
