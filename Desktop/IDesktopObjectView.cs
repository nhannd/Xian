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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines a base interface for views that serve desktop objects.
    /// </summary>
    /// <remarks>
	/// The view provides the on-screen representation of the object.
	/// </remarks>
    public interface IDesktopObjectView : IView, IDisposable
    {
        /// <summary>
        /// Occurs when the <see cref="Visible"/> property changes.
        /// </summary>
        event EventHandler VisibleChanged;

        /// <summary>
        /// Occurs when the <see cref="Active"/> property changes.
        /// </summary>
        event EventHandler ActiveChanged;

        /// <summary>
        /// Occurs when the user has requested that the object be closed.
        /// </summary>
        event EventHandler CloseRequested;

        /// <summary>
        /// Sets the title that is displayed to the user.
        /// </summary>
        void SetTitle(string title);

        /// <summary>
        /// Opens the view (makes it first visible on the screen).
        /// </summary>
        void Open();

        /// <summary>
        /// Shows the view.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the view.
        /// </summary>
        void Hide();

        /// <summary>
        /// Activates the view.
        /// </summary>
        void Activate();

        /// <summary>
        /// Gets a value indicating whether the view is visible on the screen.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets a value indicating whether the view is active.
        /// </summary>
        bool Active { get; }
    }
}
