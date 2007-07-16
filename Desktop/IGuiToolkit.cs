using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface for bootstrapping a GUI subsystem such as Windows Forms or GTK.
    /// </summary>
    public interface IGuiToolkit
    {
        /// <summary>
        /// Gets the ID of the toolkit.
        /// </summary>
        string ToolkitID { get; }

        /// <summary>
        /// Initializes the toolkit.  This method is called prior <see cref="RunMessagePump"/>.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Starts the message pump of the underlying GUI toolkit.  It is expected that this method will
        /// block for the duration of the application execution.
        /// </summary>
        /// <remarks>
        /// The method assumes that the view relies on an underlying message pump, as most 
        /// desktop GUI toolkits do.
        /// </remarks>
        void RunMessagePump();

        /// <summary>
        /// Terminates the message pump of the underlying GUI toolkit, prior to termination of the application.
        /// </summary>
        void QuitMessagePump();
    }
}
