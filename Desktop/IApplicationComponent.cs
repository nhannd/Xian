#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Indicates the exit status of an application component.
    /// </summary>
    public enum ApplicationComponentExitCode
    {
        /// <summary>
        /// Implies that nothing of significance occured; the component was closed or cancelled.
        /// </summary>
        None,

        /// <summary>
        /// For an editable component, implies that data was changed and the user accepted the changes.
        /// </summary>
        Accepted,

        /// <summary>
        /// An error occured during the component execution.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Defines the interface to an application component as seen by an application component host.
    /// </summary>
    /// <remarks>
    /// An application component must implement this interface in order to be hosted by the desktop framework.
    /// </remarks>
    public interface IApplicationComponent
    {
        /// <summary>
        /// Called by the framework to initialize the component with a host.
        /// </summary>
        void SetHost(IApplicationComponentHost host);

        /// <summary>
        /// Allows the component to export a set of actions to the host.
        /// </summary>
        /// <remarks>
        /// It is up to the host implementation to determine what, if anything,
        /// is done with the actions.
		/// </remarks>
        IActionSet ExportedActions { get; }

        /// <summary>
        /// Called by the framework to initialize the component.
        /// </summary>
        /// <remarks>
		/// This method will be called before the component becomes visible
		/// on the screen.  All significant initialization should be performed
		/// here rather than in the constructor.
		/// </remarks>
        void Start();

        /// <summary>
        /// Called by the framework to allow the component to perform any clean-up.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns true if the component is live.
        /// </summary>
        /// <remarks>
		/// A component is considered live after the Start()
		/// method has been called, and before the Stop() method is called.
		/// </remarks>
        bool IsStarted { get; }

        /// <summary>
        /// Allows the host to determine whether this component holds modified
        /// data that may need to be saved.
        /// </summary>
        bool Modified { get; }

        /// <summary>
        /// Notifies the host that the value of the <see cref="Modified"/> property has changed.
        /// </summary>
        event EventHandler ModifiedChanged;

        /// <summary>
        /// Notifies the host that the value of any or all properties may have changed.
        /// </summary>
        event EventHandler AllPropertiesChanged;

        /// <summary>
        /// Gets a value indicating whether there are any validation errors based on the current state of the component.
        /// </summary>
        bool HasValidationErrors { get; }

        /// <summary>
        /// Shows or hides validation errors.
        /// </summary>
        void ShowValidation(bool show);

        /// <summary>
        /// Gets a value indicating whether validation errors should be visible on the user-interface.
        /// </summary>
        bool ValidationVisible { get; }

        /// <summary>
        /// Occurs when the <see cref="ValidationVisible"/> property has changed.
        /// </summary>
        event EventHandler ValidationVisibleChanged;

        /// <summary>
        /// Called by the framework to determine if this component in a state
        /// such that it can be stopped without user interaction.
        /// </summary>
        bool CanExit();

        /// <summary>
        /// Called by the framework in the case where the host has initiated the exit, rather than the component,
        /// to give the component a chance to prepare prior to being stopped.
        /// </summary>
        /// <returns>Whether or not the component is capable of exiting at this time.</returns>
        bool PrepareExit();

        /// <summary>
        /// Gets or sets the exit code for the component.
        /// </summary>
        ApplicationComponentExitCode ExitCode { get; }
    }
}
