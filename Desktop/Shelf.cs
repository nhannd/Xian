using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a shelf within a desktop window.
    /// </summary>
    public class Shelf : DesktopObject, IShelf
    {
        #region Host Implementation

        // implements the host interface, which is exposed to the hosted application component
        class Host : ApplicationComponentHost
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

            public override void SetTitle(string title)
            {
                _shelf.Title = title;
            }
        }

        #endregion

        private Host _host;
        private DesktopWindow _desktopWindow;
        private ShelfDisplayHint _displayHint;
        private bool _exitRequestedByComponent;

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

        IDesktopWindow IShelf.DesktopWindow
        {
            get { return _desktopWindow; }
        }

        #endregion

        #region Protected overrides

        protected override void Initialize()
        {
            _host.StartComponent();
            base.Initialize();
        }

        protected internal override bool CanClose(UserInteraction interactive)
        {
            return _exitRequestedByComponent || _host.Component.CanExit(interactive);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }

        protected override IDesktopObjectView CreateView()
        {
            return _desktopWindow.CreateShelfView(this);
        }

        #endregion

        #region Helpers

        protected IShelfView ShelfView
        {
            get { return (IShelfView)this.View; }
        }

        #endregion

    }
}
