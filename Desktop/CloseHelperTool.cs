using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides "Close Assistant" services, which inform the user of workspaces that require attention prior
    /// to a desktop window close or application quit.
    /// </summary>
    [ExtensionOf(typeof(ApplicationToolExtensionPoint))]
    class CloseHelperTool : Tool<IApplicationToolContext>
    {
        private Shelf _closeHelperShelf;

        public CloseHelperTool()
        {

        }

        public override void Initialize()
        {
            Application.Quitting += new EventHandler<QuittingEventArgs>(Application_Quitting);
            Application.DesktopWindows.ItemClosing += new EventHandler<ClosingItemEventArgs<DesktopWindow>>(Windows_ItemClosing);

            base.Initialize();
        }

        void Windows_ItemClosing(object sender, ClosingItemEventArgs<DesktopWindow> e)
        {
            // if application is quitting, don't do anything here because it will be handled by the Quitting handler
            if (e.Interaction != UserInteraction.Allowed || e.Cancel || e.Reason == CloseReason.ApplicationQuit)
                return;

            // find all the workspaces that can't be closed
            DesktopWindow window = e.Item;
            bool showHelper = CollectionUtils.Contains<Workspace>(window.Workspaces,
                delegate(Workspace w) { return !w.QueryCloseReady(); });

            if (showHelper)
            {
                e.Cancel = true;
                ShowShelf(window, e.Reason);
            }
        }

        void Application_Quitting(object sender, QuittingEventArgs e)
        {
            if (e.Interaction != UserInteraction.Allowed || e.Cancel)
                return;

            // check if we need to show the shelf
            bool showHelper = CollectionUtils.Contains<DesktopWindow>(Application.DesktopWindows,
                delegate(DesktopWindow w) { return !w.QueryCloseReady(); });

            if (showHelper)
            {
                e.Cancel = true;
                ShowShelf(Application.ActiveDesktopWindow, CloseReason.ApplicationQuit);
            }
        }

        private void ShowShelf(DesktopWindow window, CloseReason reason)
        {
            if (_closeHelperShelf != null && _closeHelperShelf.DesktopWindow != window)
            {
                // the shelf is in another window, close it
                _closeHelperShelf.Close();
                _closeHelperShelf = null;
            }

            // the shelf is not currently open
            if (_closeHelperShelf == null)
            {
                // launch it
                CloseHelperComponent component = new CloseHelperComponent();
                _closeHelperShelf = ApplicationComponent.LaunchAsShelf(window, component, "Close Assistant",
                    ShelfDisplayHint.DockLeft, delegate(IApplicationComponent c) { _closeHelperShelf = null; });
            }

            CloseHelperComponent helper = (CloseHelperComponent)_closeHelperShelf.Component;
            helper.Refresh(reason != CloseReason.ApplicationQuit);
        }
    }
}
