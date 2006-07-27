using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Indicates the exit status of an application component.
    /// </summary>
    public enum ApplicationComponentExitCode
    {
        /// <summary>
        /// The component exited normally.  If the component allows editing,
        /// this typically means that the user accepted the changes.
        /// </summary>
        Normal,

        /// <summary>
        /// The component was cancelled.  If the component allows editing,
        /// this typically means that the user cancelled the changes.
        /// </summary>
        Cancelled,
    }

    /// <summary>
    /// Defines the interface to an application component as seen by
    /// the host.  All methods on this interface are intended solely
    /// for use by the application component host.
    /// </summary>
    public interface IApplicationComponent
    {
        /// <summary>
        /// Called by the framework to initialize the component with a host.
        /// </summary>
        /// <param name="host"></param>
        void SetHost(IApplicationComponentHost host);

        /// <summary>
        /// Allows the component to expose a public set of tools.  Some types
        /// of host may expose these tools in the user-interface.
        /// </summary>
        IToolSet ToolSet { get; }

        /// <summary>
        /// Called by the framework to initialize the component.  This method
        /// is guaranteed to be called before the component becomes visible
        /// on the screen.  All significant initialization should be performed
        /// here rather than in the constructor.
        /// </summary>
        void Start();

        /// <summary>
        /// Called by the framework to allow the component to perform any
        /// clean-up.
        /// </summary>
        void Stop();

        /// <summary>
        /// Allows the host to determine whether this component holds modified
        /// data that may need to be saved.
        /// </summary>
        bool Modified { get; }

        /// <summary>
        /// Notifies that the value of the <see cref="Modified"/> property has changed.
        /// </summary>
        event EventHandler ModifiedChanged;

        /// <summary>
        /// Called by the framework to determine if this component in a state
        /// such that it can be stopped.
        /// </summary>
        /// <remarks>
        /// Within the implementation of this method,
        /// the component is free to perform any necessary interaction with the 
        /// user, such as the display of a confirmation dialog, to determine
        /// whether it is appropriate to exit.  The component should also be sure to 
        /// set the value of <see cref="ExitCode"/> before returning from this method.
        /// 
        /// Note that if the component itself requests the exit (by calling
        /// the <see cref="IApplicationComponentHost.Exit"/> method), then this method
        /// will not be called, since it is assumed that the component is in a suitable
        /// state.
        /// </remarks>
        /// <returns>True if the component is ready to exit</returns>
        bool CanExit();

        /// <summary>
        /// A value that is returned to the caller after the component exits,
        /// indicating the circumstances of the exit.
        /// </summary>
        ApplicationComponentExitCode ExitCode { get; }
    }
}
