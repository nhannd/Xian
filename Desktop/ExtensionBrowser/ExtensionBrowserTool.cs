using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.ExtensionBrowser.PluginView;
using ClearCanvas.Desktop.ExtensionBrowser.ExtensionPointView;

namespace ClearCanvas.Desktop.ExtensionBrowser
{
	[MenuAction("show", "global-menus/MenuTools/MenuUtilities/MenuExtensionBrowser", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.Control | XKeys.E)]
    [ClickHandler("show", "Show")]
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
                    ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide,
                    delegate(IApplicationComponent component) { _shelf = null; });
            }
        }
    }
}
