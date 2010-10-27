#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;


namespace ClearCanvas.Desktop.Applets.WebBrowser
{
	[ButtonAction("activate", "webbrowser-toolbar/Google", "LaunchGoogle")]
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
