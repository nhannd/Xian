using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface for an extension of <see cref="GuiToolkitExtensionPoint"/>.
    /// <remarks>
    /// One extension must exist or the desktop application will not run.
    /// The purpose of the extension is to bootstrap a GUI subsystem such as Windows Forms or GTK.
    /// </remarks>
    /// </summary>
    public interface IGuiToolkit
    {
        /// <summary>
        /// Gets the ID of the toolkit.
        /// </summary>
        string ToolkitID { get; }

        /// <summary>
        /// Occurs when the toolkit has successfully started (e.g. its message loop is active).
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Initializes the GUI toolkit and starts the internal message loop,
        /// blocking until <see cref="Terminate"/> is called.
        /// </summary>
        /// <remarks>
        /// This method must block until <see cref="Terminate"/> is called.  This method must also ensure
        /// that the <see cref="Started"/> event is raised from within the message loop of the GUI system.
        /// </remarks>
        void Run();

        /// <summary>
        /// Terminates the GUI toolkit, shutting down the internal message loop and releasing the
        /// blocked <see cref="Run"/> method.
        /// </summary>
        void Terminate();
    }
}
