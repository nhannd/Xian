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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a shelf within a desktop window.
    /// </summary>
    public class Shelf : DesktopObject, IShelf
    {
        #region Host Implementation

        // implements the host interface, which is exposed to the hosted application component
        private class Host : ApplicationComponentHost, IShelfHost
        {
            private Shelf _shelf;

            internal Host(Shelf shelf, IApplicationComponent component)
                : base(component)
            {
                Platform.CheckForNullReference(shelf, "shelf");
                _shelf = shelf;
            }

            public override void Exit()
            {
                _shelf._exitRequestedByComponent = true;
                // close the shelf
                _shelf.Close(UserInteraction.Allowed, CloseReason.Program);
            }

            public override DesktopWindow DesktopWindow
            {
                get { return _shelf._desktopWindow; }
            }

            public override string Title
            {
                get { return _shelf.Title; }
                set { _shelf.Title = value; }
            }
        }

        #endregion

        private Host _host;
        private DesktopWindow _desktopWindow;
        private ShelfDisplayHint _displayHint;
        private bool _exitRequestedByComponent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args">Object used to specify how the <see cref="Shelf"/> should be created.</param>
        /// <param name="desktopWindow">The owner window of the <see cref="Shelf"/>.</param>
        protected internal Shelf(ShelfCreationArgs args, DesktopWindow desktopWindow)
            :base(args)
        {
            _desktopWindow = desktopWindow;
            _displayHint = args.DisplayHint;
            _host = new Host(this, args.Component);
        }

        #region Public properties

        /// <summary>
        /// Gets the hosted component.
        /// </summary>
        public object Component
        {
            get { return _host.Component; }
        }

        /// <summary>
        /// Gets the desktop window that owns this shelf.
        /// </summary>
        public DesktopWindow DesktopWindow
        {
            get { return _desktopWindow; }
        }

        /// <summary>
        /// Gets the current display hint.
        /// </summary>
        public ShelfDisplayHint DisplayHint
        {
            get { return _displayHint; }
            protected set { _displayHint = value; }
        }
        
        #endregion

        #region Public methods

        /// <summary>
        /// Makes the shelf visible.
        /// </summary>
        public void Show()
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open, DesktopObjectState.Closing });
            
            this.ShelfView.Show();
        }

        /// <summary>
        /// Hides the shelf from view.
        /// </summary>
        public void Hide()
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open, DesktopObjectState.Closing });

            this.ShelfView.Hide();
        }

        #endregion

        #region IShelf Members

		/// <summary>
		/// Gets the owner <see cref="IDesktopWindow"/>.
		/// </summary>
        IDesktopWindow IShelf.DesktopWindow
        {
            get { return _desktopWindow; }
        }

        #endregion

        #region Protected overrides

        /// <summary>
        /// Starts the hosted component.
        /// </summary>
        protected override void Initialize()
        {
            _host.StartComponent();
            base.Initialize();
        }

        /// <summary>
        /// Checks if the hosted component can exit.
        /// </summary>
        /// <returns></returns>
        protected internal override bool CanClose()
        {
            return _exitRequestedByComponent || _host.Component.CanExit();
        }

        /// <summary>
        /// Gives the hosted component a chance to prepare for a forced exit.
        /// </summary>
        protected override bool PrepareClose(CloseReason reason)
        {
            base.PrepareClose(reason);

            return _exitRequestedByComponent || _host.Component.PrepareExit();
        }
        
        /// <summary>
        /// Disposes of this object, stopping the hosted component.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }

        /// <summary>
        /// Creates a view for this shelf.
        /// </summary>
        /// <returns></returns>
        protected sealed override IDesktopObjectView CreateView()
        {
            return _desktopWindow.CreateShelfView(this);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the view for this object as a <see cref="IShelfView"/>.
        /// </summary>
        protected IShelfView ShelfView
        {
            get { return (IShelfView)this.View; }
        }

        #endregion

    }
}
