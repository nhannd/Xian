#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to an application component view as seen by the application component host.
    /// </summary>
    public interface IApplicationComponentView : IView
    {
        /// <summary>
        /// Called by the host to assign this view to a component.
        /// </summary>
        /// <param name="component">The component that this view should serve.</param>
        void SetComponent(IApplicationComponent component);
    }
}
