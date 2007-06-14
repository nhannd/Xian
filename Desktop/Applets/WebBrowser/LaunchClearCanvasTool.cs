using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;


namespace ClearCanvas.Desktop.Applets.WebBrowser
{
	[ButtonAction("activate1", "webbrowser-toolbar/ClearCanvas")]
	[ClickHandler("activate1", "LaunchClearCanvas")]
	[Tooltip("activate1", "Launch ClearCanvas")]
	[IconSet("activate1", IconScheme.Colour, "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png")]

	[ButtonAction("activate2", "webbrowser-toolbar/Discussion Forum")]
	[ClickHandler("activate2", "LaunchDiscussionForum")]
	[Tooltip("activate2", "Launch ClearCanvas Discussion Forum")]
	[IconSet("activate2", IconScheme.Colour, "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png")]

	[ExtensionOf(typeof(WebBrowserToolExtensionPoint))]
	public class LaunchClearCanvasTool : Tool<IWebBrowserToolContext>
	{
		public LaunchClearCanvasTool()
		{

		}

		private void LaunchClearCanvas()
		{
			this.Context.Url = "http://www.clearcanvas.ca";
			this.Context.Go();
		}

		private void LaunchDiscussionForum()
		{
			this.Context.Url = "http://www.clearcanvas.ca/forum/index.php";
			this.Context.Go();
		}
	}
}
