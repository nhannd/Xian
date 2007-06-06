using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;


namespace ClearCanvas.ImageViewer.Samples.WebBrowser
{
	[ButtonAction("activate", "webbrowser-toolbar/Google")]
	[ClickHandler("activate", "LaunchGoogle")]
	//[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "Launch Google")]
	[IconSet("activate", IconScheme.Colour, "Icons.GoogleToolSmall.png", "Icons.GoogleToolSmall.png", "Icons.GoogleToolSmall.png")]
	[ExtensionOf(typeof(WebBrowserToolExtensionPoint))]
	public class LaunchGoogleTool : Tool<IWebBrowserToolContext>
	{
		public LaunchGoogleTool()
		{

		}

		private void LaunchGoogle()
		{
			this.Context.Url = "http://google.com";
			this.Context.Go();
		}
	}
}
