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

        event EventHandler Started;

        /// <summary>
        /// Runs the GUI toolkits internal message loop, blocking until <see cref="Terminate"/> is called.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        void Run();

        void Terminate();
    }
}
