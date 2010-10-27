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
	[ButtonAction("activate1", "webbrowser-toolbar/ClearCanvas", "LaunchClearCanvas")]
	[Tooltip("activate1", "Launch ClearCanvas")]
	[IconSet("activate1", IconScheme.Colour, "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png")]

	[ButtonAction("activate2", "webbrowser-toolbar/Discussion Forum", "LaunchDiscussionForum")]
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
			this.Context.Url = "http://www.clearcanvas.ca/dnn/Community/Forums/tabid/69/Default.aspx";
			this.Context.Go();
		}
	}
}
