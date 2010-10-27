#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	internal class DesktopWindowTool : Tool<IDesktopToolContext>
	{
		public DesktopWindowTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			KeyImageClipboard.OnDesktopWindowOpened(Context.DesktopWindow);
		}

		protected override void Dispose(bool disposing)
		{
			if (Context != null)
				KeyImageClipboard.OnDesktopWindowClosed(Context.DesktopWindow);

			base.Dispose(disposing);
		}
	}
}
