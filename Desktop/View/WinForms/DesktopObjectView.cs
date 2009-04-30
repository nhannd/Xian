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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Abstract base implementation of <see cref="IDesktopObjectView"/>.
    /// </summary>
    public abstract class DesktopObjectView : WinFormsView, IDesktopObjectView
    {
        private bool _active;
        private bool _visible;
        private event EventHandler _activeChanged;
        private event EventHandler _visibleChanged;
        private event EventHandler _closeRequested;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DesktopObjectView()
        {
        }
        
        #region IDesktopObjectView Members

        /// <summary>
        /// Occurs when the <see cref="Visible"/> property changes.
        /// </summary>
        public event EventHandler VisibleChanged
        {
            add { _visibleChanged += value; }
            remove { _visibleChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="Active"/> property changes.
        /// </summary>
        public event EventHandler ActiveChanged
        {
            add { _activeChanged += value; }
            remove { _activeChanged -= value; }
        }

        /// <summary>
        /// Occurs when the user has requested to close the view.
        /// </summary>
        public event EventHandler CloseRequested
        {
            add { _closeRequested += value; }
            remove { _closeRequested -= value; }
        }

        /// <summary>
        /// Sets the title that is displayed on the view.
        /// </summary>
        /// <param name="title"></param>
        public abstract void SetTitle(string title);

        /// <summary>
        /// Opens the view.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Shows the view.
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// Hides the view.
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// Activates the view.
        /// </summary>
        public abstract void Activate();

        /// <summary>
        /// Gets a value indicating whether the view is visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
        }

        /// <summary>
        /// Gets a value indicating whether the view is active.
        /// </summary>
        public bool Active
        {
            get { return _active; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Sets the <see cref="Visible"/> property of this view.
        /// </summary>
        /// <param name="visible"></param>
        protected internal void SetVisibleStatus(bool visible)
        {
            if (_visible != visible)
            {
                _visible = visible;
                OnVisibleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the <see cref="Active"/> property of this view.
        /// </summary>
        /// <param name="active"></param>
        protected internal void SetActiveStatus(bool active)
        {
            if (_active != active)
            {
                _active = active;
                OnActiveChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="CloseRequested"/> event.
        /// </summary>
        protected internal void RaiseCloseRequested()
        {
            OnCloseRequested(EventArgs.Empty);
        }

        #endregion

        #region Protected overridables

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // nothing to dispose of
        }

        /// <summary>
        /// Raises the <see cref="CloseRequested"/> event.
        /// </summary>
        protected virtual void OnCloseRequested(EventArgs e)
        {
            EventsHelper.Fire(_closeRequested, this, e);
        }

        /// <summary>
        /// Raises the <see cref="ActiveChanged"/> event.
        /// </summary>
        protected virtual void OnActiveChanged(EventArgs e)
        {
            EventsHelper.Fire(_activeChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="VisibleChanged"/> event.
        /// </summary>
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            EventsHelper.Fire(_visibleChanged, this, e);
        }

        #endregion

        #region WinFormsView overrides

        /// <summary>
        /// Not used by this class.
        /// </summary>
        public override object GuiElement
        {
            // not used
            get { throw new NotSupportedException(); }
        }

        #endregion
    }
}
