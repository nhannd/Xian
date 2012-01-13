#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ButtonAction("apply", "global-toolbars/Performance Analysis", "Apply")]
	[IconSet("apply", "PerformanceAnalysisToolSmall.png", "PerformanceAnalysisToolMedium.png", "PerformanceAnalysisToolLarge.png")]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class PerformanceAnalysisTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

		public PerformanceAnalysisTool()
		{
		}

		public void Apply()
		{
			if (_shelf != null)
			{
				_shelf.Activate();
				return;
			}

			PerformanceAnalysisComponent component = new PerformanceAnalysisComponent();
			_shelf = ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, component, "Performance Analysis",
			                                   ShelfDisplayHint.DockFloat);

			_shelf.Closing += delegate { _shelf = null; };
		}
	}
}
