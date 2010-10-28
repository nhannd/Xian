#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an interface that must be implemented by application component containers (components
    /// that host other components).
    /// </summary>
    public interface IApplicationComponentContainer
    {
        /// <summary>
        /// Gets all contained components.
        /// </summary>
        IEnumerable<IApplicationComponent> ContainedComponents { get; }

        /// <summary>
        /// Get the contained components that are currently visible to the user.
        /// </summary>
        IEnumerable<IApplicationComponent> VisibleComponents { get; }

        /// <summary>
        /// Ensures that the specified component is made visible to the user.
        /// </summary>
        void EnsureVisible(IApplicationComponent component);

        /// <summary>
        /// Ensures that the specified component has been started. 
        /// </summary>
        /// <remarks>
        /// A container may choose not to start components until they are actually displayed for the first time.
        /// This method ensures that a component is started regardless of whether it has ever been displayed.
        /// This is necessary, for instance, if the component is to be validated as part of validating the container.
        /// </remarks>
        void EnsureStarted(IApplicationComponent component);
    }
}
