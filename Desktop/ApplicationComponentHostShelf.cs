using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Define an extension point for a view onto this shelf
    /// </summary>
    [ExtensionPoint]
    public class ApplicationComponentHostShelfViewExtensionPoint : ExtensionPoint<IShelfView>
    {
    }

    /// <summary>
    /// Hosts an application component in a shelf.  See <see cref="ApplicationComponent.LaunchAsShelf"/>.
    /// </summary>
    [AssociateView(typeof(ApplicationComponentHostShelfViewExtensionPoint))]
    public class ApplicationComponentHostShelf : Shelf
    {
        // implements the host interface, which is exposed to the hosted application component
        class Host : IApplicationComponentHost
        {
            private ApplicationComponentHostShelf _shelf;

            internal Host(ApplicationComponentHostShelf shelf)
            {
				Platform.CheckForNullReference(shelf, "shelf");
                _shelf = shelf;
            }

            public void Exit()
            {
                // close the shelf
                _shelf.DesktopWindow.ShelfManager.Shelves.Remove(_shelf);
            }

            public DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
            {
                return Platform.ShowMessageBox(message, buttons);
            }

            public CommandHistory CommandHistory
            {
                get
                {
                    // this could possibly be implemented in future, if the need arises
                    // however, it is not clear which command history would actually be used,
                    // since there is no global command history
                    throw new NotSupportedException();
                }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _shelf.DesktopWindow; }
            }
        }

        private IApplicationComponent _component;
        private ApplicationComponentExitDelegate _exitCallback;


        internal ApplicationComponentHostShelf(string title, IApplicationComponent component, ShelfDisplayHint hint,
            ApplicationComponentExitDelegate exitCallback)
            :base(title, hint)
        {
			Platform.CheckForNullReference(component, "component");
            _component = component;
            _exitCallback = exitCallback;

            _component.SetHost(new Host(this));
        }

        /// <summary>
        /// Gets the hosted component
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        #region IShelf Members

        public override void Initialize(IDesktopWindow desktopWindow)
        {
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            base.Initialize(desktopWindow);
            _component.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _component != null)
            {
                _component.Stop();

                if (_exitCallback != null)
                {
                    _exitCallback(_component);
                }

                _component = null;
            }
        }

        #endregion
    }
}
