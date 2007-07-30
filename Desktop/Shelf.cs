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
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        /// <param name="desktopWindow"></param>
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
        /// <param name="interactive"></param>
        /// <returns></returns>
        protected internal override bool CanClose(UserInteraction interactive)
        {
            return _exitRequestedByComponent || _host.Component.CanExit(interactive);
        }

        /// <summary>
        /// Disposes of this object, stopping the hosted component.
        /// </summary>
        /// <param name="disposing"></param>
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
