#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
#if DEBUG
	[MenuAction("apply", "global-menus/MenuTools/MenuUtilities/MenuMemoryUsage", "Apply")]
	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
#endif
	public class MemoryUsageTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public MemoryUsageTool()
		{
		}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Apply()
		{
			long bytesUsed = GC.GetTotalMemory(false);
			string str = String.Format("Memory used: {0:N}", bytesUsed);
			this.Context.DesktopWindow.ShowMessageBox(str, MessageBoxActions.Ok);
		}
	}
}
