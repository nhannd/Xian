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
        class Host : ApplicationComponentHost
        {
            private ApplicationComponentHostShelf _shelf;

            internal Host(ApplicationComponentHostShelf shelf, IApplicationComponent component, ApplicationComponentExitDelegate exitCallback)
                :base(component, exitCallback)
            {
				Platform.CheckForNullReference(shelf, "shelf");
                _shelf = shelf;
            }

            public override void Exit()
            {
                // close the shelf
                _shelf.DesktopWindow.ShelfManager.Shelves.Remove(_shelf);
            }

            public override IDesktopWindow DesktopWindow
            {
                get { return _shelf.DesktopWindow; }
            }

            public override void SetTitle(string title)
            {
                _shelf.Title = title;
            }
        }

        private Host _host;


        internal ApplicationComponentHostShelf(IDesktopWindow desktopWindow,
            string title, IApplicationComponent component, ShelfDisplayHint hint,
            ApplicationComponentExitDelegate exitCallback)
            :base(title, hint, desktopWindow)
        {
			Platform.CheckForNullReference(component, "component");
            _host = new Host(this, component, exitCallback);
            _host.StartComponent();
        }

        /// <summary>
        /// Gets the hosted component
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _host.Component; }
        }

        public override void Initialize(IDesktopWindow desktopWindow)
        {
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            base.Initialize(desktopWindow);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }
    }
}
