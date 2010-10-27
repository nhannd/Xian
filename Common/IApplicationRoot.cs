#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common
{
    /// <summary>
    /// Defines an application entry point.
    /// </summary>
    /// <seealso cref="ApplicationRootExtensionPoint"/>
    public interface IApplicationRoot
    {
        /// <summary>
        /// Called by the platform to run the application.
        /// </summary>
        /// <remarks>
        /// It is expected that this method may block for the duration of the application's execution, if
        /// for instance, a GUI event message pump is started.
        /// </remarks>
        void RunApplication(string[] args);
    }
}
