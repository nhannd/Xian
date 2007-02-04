using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Controls;
using System.ComponentModel;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class ShelfViewManager
    {
        private ShelfManager _shelfManager;
        private DockingManager _dockingManager;
        private Dictionary<IShelf, Content> _shelfViewMap;

        public ShelfViewManager(ShelfManager shelfManager, DockingManager dockingManager)
		{
            _shelfViewMap = new Dictionary<IShelf, Content>();

            _shelfManager = shelfManager;
            _shelfManager.Shelves.ItemAdded += new EventHandler<ShelfEventArgs>(Shelves_ItemAdded);
            _shelfManager.Shelves.ItemRemoved += new EventHandler<ShelfEventArgs>(Shelves_ItemRemoved);

			_dockingManager = dockingManager;

			// NY: We subscribe to ContentHiding instead of ContentHidden because ContentHidden
			// is fired when the user double clicks the caption bar of a docking window, which
			// results in a crash. (Ticket #144)
			_dockingManager.ContentHiding += new DockingManager.ContentHidingHandler(_dockingManager_ContentHiding);
		}


        public void HideShelves()
        {
            // 1) Retract all visible autohide windows
            // 2) Put docked windows in autohide mode if the tool has specified so
            _dockingManager.RemoveShowingAutoHideWindows();

            for (int i = 0; i < _dockingManager.Contents.Count; i++)
            {
                Content content = _dockingManager.Contents[i];

                IShelf shelf = (IShelf)content.Tag;
                if ((shelf.DisplayHint & ShelfDisplayHint.HideOnWorkspaceOpen) != 0)
                {
                    if (!content.IsAutoHidden)
                        _dockingManager.ToggleContentAutoHide(content);
                }
            }
        }

        private void Shelves_ItemAdded(object sender, ShelfEventArgs e)
        {
            AddShelfView(e.Item);
        }

        private void Shelves_ItemRemoved(object sender, ShelfEventArgs e)
        {
            RemoveShelfView(e.Item);
        }

		void _dockingManager_ContentHiding(Content c, CancelEventArgs cea)
		{
			IShelf shelf = (IShelf)c.Tag;
			_shelfManager.Shelves.Remove(shelf);
		}
		
        private void AddShelfView(IShelf shelf)
        {
            IShelfView view = (IShelfView)ViewFactory.CreateAssociatedView(shelf.GetType());
            view.SetShelf(shelf);

            Content content = _dockingManager.Contents.Add((Control)view.GuiElement, shelf.Title);
            content.Tag = shelf;

            ShelfDisplayHint hint = shelf.DisplayHint;

            // Make sure the window is the size as it's been defined by the tool
            if ((hint & ShelfDisplayHint.MaximizeOnDock) != 0)
                content.DisplaySize = _dockingManager.Container.Size;
            else
                content.DisplaySize = content.Control.Size;

            content.AutoHideSize = content.Control.Size;
            content.FloatingSize = content.Control.Size;
            //content.CloseOnHide = false;
            content.Tag = shelf;

            if ((hint & ShelfDisplayHint.DockAutoHide) != 0)
                _dockingManager.Container.SuspendLayout();

            // Dock the window on the correct edge
            if ((hint & ShelfDisplayHint.DockTop) != 0)
                _dockingManager.AddContentWithState(content, State.DockTop);
            else if ((hint & ShelfDisplayHint.DockBottom) != 0)
                _dockingManager.AddContentWithState(content, State.DockBottom);
            else if ((hint & ShelfDisplayHint.DockLeft) != 0)
                _dockingManager.AddContentWithState(content, State.DockLeft);
            else if ((hint & ShelfDisplayHint.DockRight) != 0)
                _dockingManager.AddContentWithState(content, State.DockRight);
            else
                _dockingManager.AddContentWithState(content, State.Floating);

            if ((hint & ShelfDisplayHint.DockAutoHide) != 0)
            {
                _dockingManager.ToggleContentAutoHide(content);
                _dockingManager.Container.ResumeLayout();
                _dockingManager.BringAutoHideIntoView(content);
            }

            _shelfViewMap.Add(shelf, content);
        }

        private void RemoveShelfView(IShelf shelf)
        {
            Content content = _shelfViewMap[shelf];
            _dockingManager.Contents.Remove(content);
            _shelfViewMap.Remove(shelf);

            // make sure to dispose of the control now (dotnetmagic doesn't do it automatically)
            if (!content.Control.IsDisposed)
            {
                content.Control.Dispose();
            }
        }

    }
}
