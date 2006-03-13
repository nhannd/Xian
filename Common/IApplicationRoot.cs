using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// This extension point serves as the application entry point.
    /// When the <see cref="Platform.StartApp"/> method is called, the platform will attempt to create
    /// exaclty one extension of IApplicationRoot.
    /// </summary>
    [ExtensionPoint()]
    public interface IApplicationRoot
    {
        /// <summary>
        /// Called by the platform to run the application, as defined by the implementing extension.
        /// </summary>
        /// <remarks>
        /// It is expected that this method may block for the duration of the application's execution, if
        /// for instance, a GUI event message pump is started.
        /// </remarks>
        void RunApplication();
    }
}
