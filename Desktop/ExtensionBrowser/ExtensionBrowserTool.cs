#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.ExtensionBrowser
{
	[MenuAction("show", "global-menus/MenuTools/MenuUtilities/MenuExtensionBrowser", "Show", Flags = ClickActionFlags.CheckAction)]
	[ActionPermission("show", AuthorityTokens.Desktop.ExtensionBrowser)]
	[GroupHint("show", "Application.Browsing.Extensions")]

    [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ExtensionBrowserTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

        public ExtensionBrowserTool()
		{
        }

        public void Show()
        {
			if (_shelf != null)
			{
				_shelf.Activate();
			}
			else
            {
				ExtensionBrowserComponent browser = new ExtensionBrowserComponent();

            	_shelf = ApplicationComponent.LaunchAsShelf(
            		this.Context.DesktopWindow,
            		browser,
            		SR.TitleExtensionBrowser,
            		"Extension Browser",
            		ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide);

				_shelf.Closed += OnShelfClosed;
            }
        }

		private void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			_shelf = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (_shelf != null)
				_shelf.Closed -= OnShelfClosed;

			base.Dispose(disposing);
		}
    }
}
