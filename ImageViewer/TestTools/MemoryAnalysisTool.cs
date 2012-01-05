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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("show", "global-menus/MenuTools/MenuUtilities/Memory Analysis", "Show", KeyStroke = XKeys.Control | XKeys.M)]
	[ButtonAction("show", "global-toolbars/ToolbarUtilities/Memory Analysis", "Show")]
	[IconSet("show", "Icons.MemoryAnalysisToolSmall.png", "Icons.MemoryAnalysisToolMedium.png", "")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class MemoryAnalysisTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private static IShelf _shelf;

		public MemoryAnalysisTool()
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
				MemoryAnalysisComponent component = new MemoryAnalysisComponent(this.Context.DesktopWindow);
				_shelf = ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, component, "Memory Analysis",
					                                   ShelfDisplayHint.DockFloat);
				_shelf.Closed += delegate { _shelf = null; };
			}
		}
	}
}
