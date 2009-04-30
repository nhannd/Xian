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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to an application component host as seen by the hosted application component.
    /// </summary>
    public interface IApplicationComponentHost
    {
        /// <summary>
        /// Instructs the host to terminate if, for instance, the user has pressed an OK or Cancel button.
        /// </summary>
        /// <remarks>
        /// The host will subsequently call <see cref="IApplicationComponent.Stop"/>.  Not all hosts
        /// support this method.
        /// </remarks>
        void Exit();

        /// <summary>
        /// Asks the host to display a message box to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="buttons">The buttons to display.</param>
        /// <returns>A result indicating which button the user pressed.</returns>
        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);

        /// <summary>
        /// Asks the host to set the title for this component in the UI.
        /// </summary>
        /// <remarks>
        /// Not all hosts support this method.
        /// </remarks>
        [Obsolete("Use the IApplicationComponentHost.Title property instead.")]
        void SetTitle(string title);

        /// <summary>
        /// Gets or sets the title that the host displays in the UI above this component.
        /// </summary>
        /// <remarks>
        /// Not all hosts support this property.
        /// </remarks>
        string Title { get; set; }

        /// <summary>
        /// Gets the <see cref="CommandHistory"/> object associated with this host.
        /// </summary>
        /// <remarks>
        /// Not all hosts support this property.
        /// </remarks>
        CommandHistory CommandHistory { get; }

        /// <summary>
        /// Gets the <see cref="DesktopWindow"/> associated with this host.
        /// </summary>
        DesktopWindow DesktopWindow { get; }
    }
}
