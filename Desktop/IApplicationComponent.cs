#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

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

        /// <summary>
        /// The component encountered an error.  If the component allows editing
        /// and is responsible for committing its own changes, this code typically
        /// indicates that the changes did not commit.
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
        /// <param name="host"></param>
        void SetHost(IApplicationComponentHost host);

        /// <summary>
        /// Allows the component to export a set of actions to the host.
        /// It is up to the host implementation to determine what, if anything,
        /// is done with the actions.
        /// </summary>
        IActionSet ExportedActions { get; }

        /// <summary>
        /// Called by the framework to initialize the component.  This method
        /// will be called before the component becomes visible
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
        /// Returns true if the component is live.  A component is considered live after the Start()
        /// method has been called, and before the Stop() method is called.
        /// </summary>
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
        /// <param name="show"></param>
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
        /// such that it can be stopped.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called with <see cref="UserInteraction.NotAllowed"/> to query the component
        /// to see if it is in a closable state.  In this case, the component must respond without interacting
        /// with the user.  The component should respond conservatively, e.g.,
        /// respond false if there is any unsaved data.
        /// </para>
        /// <para>
        /// The method is called with <see cref="UserInteraction.Allowed"/> to allow the component to prepare to exit.
        /// In this case, the component is free to perform any necessary interaction with the 
        /// user, such as the display of a confirmation dialog, to determine
        /// whether it is appropriate to exit.  
        /// </para>
        /// <para>
        /// If the component returns true, it should also be sure to 
        /// set the value of <see cref="ExitCode"/> before returning.
        /// </para>
        /// <para>
        /// Note that if the component itself requests the exit (by calling
        /// the <see cref="IApplicationComponentHost.Exit"/> method), then this method
        /// will not be called, since it is assumed that the component is prepared to be stopped.
        /// </para>
        /// </remarks>
        /// <returns>True if the component is ready to exit.</returns>
        bool CanExit(UserInteraction allowInteraction);

        /// <summary>
        /// Gets or sets the exit code for the component.
        /// </summary>
        ApplicationComponentExitCode ExitCode { get; set; }
    }
}
