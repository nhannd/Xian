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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines possible reasons why a <see cref="DesktopObject"/> might close.
    /// </summary>
    [Flags]
    public enum CloseReason
    {
        /// <summary>
        /// The close request was initiated by the user via the user-interface.
        /// </summary>
        UserInterface = 0x0001,

        /// <summary>
        /// The close request was initiated by the application.
        /// </summary>
        Program = 0x0002,

        /// <summary>
        /// The close request is occuring because the application has been asked to terminate.
        /// </summary>
        /// <remarks>
        /// The <see cref="Application.Quit"/> API may have been invoked, or the request
        /// may have come from the operating system.
		/// </remarks>
        ApplicationQuit = 0x0004,

        /// <summary>
        /// The object is being closed because it's parent window is closing.
        /// </summary>
		/// <remarks>
		/// Applicable to <see cref="Workspace"/> and <see cref="Shelf"/> objects.
		/// This value is combined with one of <see cref="UserInterface"/>, 
		/// <see cref="Program"/> or <see cref="ApplicationQuit"/>, indicating why the parent is closing.
		/// </remarks>
		ParentClosing = 0x0010

    }

    /// <summary>
    /// Provides data for Closing events, where the request may need to be cancelled.
    /// </summary>
    public class ClosingEventArgs : EventArgs
    {
        private CloseReason _reason;
        private bool _cancel;
        private UserInteraction _interaction;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="reason">The reason the <see cref="DesktopObject"/> is closing.</param>
		/// <param name="interaction">The user interaction policy for the closing object.</param>
		internal ClosingEventArgs(CloseReason reason, UserInteraction interaction)
            : this(reason, interaction, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="reason">The reason the <see cref="DesktopObject"/> is closing.</param>
		/// <param name="interaction">The user interaction policy for the closing object.</param>
		/// <param name="cancel">A boolean value indicating whether the close operation should be cancelled.</param>
		internal ClosingEventArgs(CloseReason reason, UserInteraction interaction, bool cancel)
        {
            _reason = reason;
            _interaction = interaction;
            _cancel = cancel;
        }

        /// <summary>
        /// Gets the reason the object is closing.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the user-interaction policy for this closing operation, which handlers must abide by.
        /// </summary>
        public UserInteraction Interaction
        {
            get { return _interaction; }
        }

        // maybe we can expose this later if needed
        internal bool Cancel
        {
            get { return _cancel; }
            set { _cancel = _cancel || value; }
        }
    }

    /// <summary>
    /// Provides data for Closing events, where the request may need to be cancelled.
    /// </summary>
    public class ClosingItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private bool _cancel;
        private CloseReason _reason;
        private UserInteraction _interaction;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="reason">The reason the <paramref name="item"/> is closing.</param>
		/// <param name="item">The item that is being closed.</param>
		/// <param name="interaction">The user interaction policy for the closing object.</param>
		/// <param name="cancel">A boolean value indicating whether the close operation should be cancelled.</param>
		internal ClosingItemEventArgs(TItem item, CloseReason reason, UserInteraction interaction, bool cancel)
            :base(item)
        {
            _reason = reason;
            _cancel = cancel;
            _interaction = interaction;
        }

        /// <summary>
        /// Gets the reason the item is closing.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the user interaction policy for this closing operation, which handlers must abide by.
        /// </summary>
        public UserInteraction Interaction
        {
            get { return _interaction; }
        }


        // maybe we can expose this later if needed
        internal bool Cancel
        {
            get { return _cancel; }
            set
            {
                // don't allow uncancelling
                _cancel = _cancel || value;
            }
        }
    }
}
