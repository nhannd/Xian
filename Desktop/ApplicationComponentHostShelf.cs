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

    [AssociateView(typeof(ApplicationComponentHostShelfViewExtensionPoint))]
    public class ApplicationComponentHostShelf : Shelf
    {
        // implements the host interface, which is exposed to the hosted application component
        class Host : IApplicationComponentHost
        {
            private ApplicationComponentHostShelf _shelf;

            internal Host(ApplicationComponentHostShelf shelf)
            {
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
            _component = component;
            _exitCallback = exitCallback;

            _component.SetHost(new Host(this));
        }

        public override void Initialize(IDesktopWindow desktopWindow)
        {
            base.Initialize(desktopWindow);
            _component.Start();
        }

        public IApplicationComponent Component
        {
            get { return _component; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _component.Stop();

                if (_exitCallback != null)
                {
                    _exitCallback(_component);
                }
            }
        }
    }
}
