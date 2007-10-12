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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for desktop objects such as windows, workspaces and shelves.
    /// </summary>
    public abstract class DesktopObject : IDesktopObject, IDisposable
    {
        private string _name;
        private string _title;
        private DesktopObjectState _state;
        private bool _active;

        private event EventHandler _opening;
        private event EventHandler _opened;
        private event EventHandler<ClosingEventArgs> _closing;
        private event EventHandler<ClosedEventArgs> _closed;

        private event EventHandler _titleChanged;
        private event EventHandler _activeChanged;
        private event EventHandler _internalActiveChanged;

        private bool _visible;
        private event EventHandler _visibleChanged;

        private IDesktopObjectView _view;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="args"></param>
        protected DesktopObject(DesktopObjectCreationArgs args)
        {
            _name = args.Name;
            _title = args.Title;
            //_visible = true;    // all objects are visible by default
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~DesktopObject()
        {
            Dispose(false);
        }

        #region Public properties

        /// <summary>
        /// Gets the runtime name of the object, or null if the object is not named.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the current state of the object.
        /// </summary>
        public DesktopObjectState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets the title that is presented to the user on the screen.
        /// </summary>
        public string Title
        {
            get { return _title; }
            protected set
            {
                if (value != _title)
                {
                    _title = value;
                    if (this.View != null)
                    {
                        this.View.SetTitle(_title);
                    }
                    OnTitleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is currently visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            private set
            {
                if (value != _visible)
                {
                    _visible = value;
                    OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is currently active.
        /// </summary>
        public bool Active
        {
            get { return _active; }
            private set
            {
                if (value != _active)
                {
                    _active = value;
                    OnInternalActiveChanged(EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Activates the object.
        /// </summary>
        public virtual void Activate()
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open });
            DoActivate();
        }

        /// <summary>
        /// Checks if the object is in a closable state (would be able to close without user interaction).
        /// </summary>
        /// <returns>True if the object can be closed without user interaction.</returns>
        public bool QueryCloseReady()
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open, DesktopObjectState.Closing });

            return CanClose(UserInteraction.NotAllowed);
        }

        /// <summary>
        /// Tries to close the object, interacting with the user if necessary.
        /// </summary>
        /// <returns>True if the object is closed, otherwise false.</returns>
        public bool Close()
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open });
            
            return Close(UserInteraction.Allowed);
        }

        /// <summary>
        /// Tries to close the object, interacting with the user only if specified.
        /// </summary>
        /// <param name="interactive">A value specifying whether user interaction is allowed.</param>
        /// <returns>True if the object is closed, otherwise false.</returns>
        public bool Close(UserInteraction interactive)
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open });

            return Close(interactive, CloseReason.Program);
        }

        #endregion

        #region Public events

        /// <summary>
        /// Occurs when the object is about to open.
        /// </summary>
        public event EventHandler Opening
        {
            add { _opening += value; }
            remove { _opening -= value; }
        }

        /// <summary>
        /// Occurs when the object has opened.
        /// </summary>
        public event EventHandler Opened
        {
            add { _opened += value; }
            remove { _opened -= value; }
        }

        /// <summary>
        /// Occurs when the object is about to close.
        /// </summary>
        public event EventHandler<ClosingEventArgs> Closing
        {
            add { _closing += value; }
            remove { _closing += value; }
        }

        /// <summary>
        /// Occurs when the object has closed.
        /// </summary>
        public event EventHandler<ClosedEventArgs> Closed
        {
            add { _closed += value; }
            remove { _closed -= value; }
        }

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
        /// Occurs when the <see cref="Title"/> property changes.
        /// </summary>
        public event EventHandler TitleChanged
        {
            add { _titleChanged += value; }
            remove { _titleChanged -= value; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
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

        #region Protected Overridables

        /// <summary>
        /// Factory method to create a view for this object.
        /// </summary>
        /// <returns></returns>
        protected abstract IDesktopObjectView CreateView();

        /// <summary>
        /// Initializes the object, prior to it becoming visible on the screen.  Override this method to perform
        /// custom initialization.
        /// </summary>
        protected virtual void Initialize()
        {
            // nothing to initialize
        }

        /// <summary>
        /// Asks the object whether it can be closed.
        /// </summary>
        /// <remarks>
        /// The default implementation just returns true. Override this method to customize the behaviour.
        /// The interaction policy indicates whether the object may interact with the user in order to determine
        /// how to respond.  If user interaction is not allowed, the object should respond conservatively
        /// (e.g. respond with false if there may be unsaved data).
        /// </remarks>
        /// <param name="interaction"></param>
        /// <returns>True if the object can be closed, otherwise false.</returns>
        protected internal virtual bool CanClose(UserInteraction interaction)
        {
            return true;
        }

        /// <summary>
        /// Gives the object an opportunity to prepare before being closed.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns>True if the object is ready to close, or false it the object cannot be closed.</returns>
        protected virtual bool PrepareClose(CloseReason reason)
        {
            // first see if we can close without interacting
            // that way we avoid calling Activate() if not necessary
            if(CanClose(UserInteraction.NotAllowed))
                return true;

            // make active, so the user is not confused if it brings up a message box
            DoActivate();

            // see if we can close with interaction
            return CanClose(UserInteraction.Allowed);
        }

        /// <summary>
        /// Called to dispose of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // view may have already been disposed in the Close method
                if (_view != null)
                {
                    _view.Dispose();
                    _view = null;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Opening"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnOpening(EventArgs args)
        {
            EventsHelper.Fire(_opening, this, args);
        }

        /// <summary>
        /// Raises the <see cref="Opened"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnOpened(EventArgs args)
        {
            EventsHelper.Fire(_opened, this, args);
        }

        /// <summary>
        /// Raises the <see cref="Closing"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnClosing(ClosingEventArgs args)
        {
            EventsHelper.Fire(_closing, this, args);
        }

        /// <summary>
        /// Raises the <see cref="Closed"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnClosed(EventArgs args)
        {
            EventsHelper.Fire(_closed, this, args);
        }

        /// <summary>
        /// Raises the <see cref="VisibleChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnVisibleChanged(EventArgs args)
        {
            EventsHelper.Fire(_visibleChanged, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ActiveChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnActiveChanged(EventArgs args)
        {
            EventsHelper.Fire(_activeChanged, this, args);
        }

        /// <summary>
        /// Raises the <see cref="TitleChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnTitleChanged(EventArgs args)
        {
            EventsHelper.Fire(_titleChanged, this, args);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Raises the <see cref="ActiveChanged"/> event.
        /// </summary>
        internal void RaiseActiveChanged()
        {
            OnActiveChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets the view for this object.
        /// </summary>
        protected IDesktopObjectView View
        {
            get { return _view; }
        }

        /// <summary>
        /// Opens this object.
        /// </summary>
        internal void Open()
        {
            // call initialize before opening
            // any exception thrown from initialize will therefore abort before opening, not after
            Initialize();

            _state = DesktopObjectState.Opening;
            OnOpening(EventArgs.Empty);

            _view = CreateView();
            _view.SetTitle(_title);
            _view.ActiveChanged += delegate(object sender, EventArgs args)
            {
                this.Active = _view.Active;
            };
            _view.VisibleChanged += delegate(object sender, EventArgs args)
            {
                this.Visible = _view.Visible;
            };
            _view.CloseRequested += delegate(object sender, EventArgs args)
            {
                // the request should always come from the active object, so interaction should be allowed
                Close(UserInteraction.Allowed, CloseReason.UserInterface);
            };

            _view.Open();

            _state = DesktopObjectState.Open;
            OnOpened(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the <see cref="Active"/> property changes.
        /// </summary>
        internal event EventHandler InternalActiveChanged
        {
            add { _internalActiveChanged += value; }
            remove { _internalActiveChanged -= value; }
        }

        /// <summary>
        /// Closes this object.
        /// </summary>
        /// <param name="interactive"></param>
        /// <param name="reason"></param>
        /// <returns>True if the object was closed, otherwise false.</returns>
        protected internal bool Close(UserInteraction interactive, CloseReason reason)
        {
            // easy case - bail if interaction is prohibited and we can't close with interacting
            if (interactive == UserInteraction.NotAllowed && !CanClose(UserInteraction.NotAllowed))
                return false;

            // either we can close without interacting, or interaction is allowed, so let's try and close

            // begin closing - the operation may yet be cancelled
            _state = DesktopObjectState.Closing;

            ClosingEventArgs args = new ClosingEventArgs(reason, interactive);
            OnClosing(args);

            if (args.Cancel || !PrepareClose(reason))
            {
                _state = DesktopObjectState.Open;
                return false;
            }

            // notify inactive
            this.Active = false;

            try
            {
                // close the view
                _view.Dispose();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
            _view = null;

            // close was successful
            _state = DesktopObjectState.Closed;
            OnClosed(new ClosedEventArgs(reason));

            // dispose of this object after firing the Closed event
            // (reason being that handlers of the Closed event may expect this object to be intact)
            (this as IDisposable).Dispose();

            return true;
        }

        /// <summary>
        /// Asserts that the object is in one of the specified valid states.
        /// </summary>
        /// <param name="validStates"></param>
        protected void AssertState(DesktopObjectState[] validStates)
        {
            if (!CollectionUtils.Contains<DesktopObjectState>(validStates,
                delegate(DesktopObjectState state) { return state == this.State; }))
            {
                string t = this.GetType().Name;
                string s = this.State.ToString();
                throw new InvalidOperationException(string.Format("Operation not valid on a {0} with State: {1}", t, s));
            }
        }

        /// <summary>
        /// Activates this object.
        /// </summary>
        private void DoActivate()
        {
            _view.Show();    // always ensure the object is visible prior to activating
            _view.Activate();
        }

        /// <summary>
        /// Raises the <see cref="InternalActiveChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        private void OnInternalActiveChanged(EventArgs args)
        {
            EventsHelper.Fire(_internalActiveChanged, this, args);
        }

        #endregion
    }
}
