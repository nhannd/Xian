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


    [MenuAction("show", "global-menus/MenuFile/MenuExtensionBrowser", Flags=ClickActionFlags.CheckAction)]
    [ClickHandler("show", "Show")]

    /// <summary>
    /// Summary description for ExtensionBrowserTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ExtensionBrowserTool : DesktopTool
	{
        private ExtensionBrowserComponent _browser;

        public ExtensionBrowserTool()
		{
        }

        public void Show()
        {
            if (_browser == null)
            {
                _browser = new ExtensionBrowserComponent();

                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    _browser,
                    SR.TitleExtensionBrowser,
                    ShelfDisplayHint.DockLeft,
                    delegate(IApplicationComponent component) { _browser = null; });
            }
        }
    }
}
