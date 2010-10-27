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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	public class RoiAnalysisComponentContainer : TabComponentContainer
	{
		private TabPage _roiHistogramPage;
		private TabPage _pathProfilePage;

		public RoiAnalysisComponentContainer(IImageViewerToolContext imageViewerToolContext)
		{
			RoiHistogramComponent roiHistogramComponent = new RoiHistogramComponent(imageViewerToolContext);
			roiHistogramComponent.Container = this;
			_roiHistogramPage = new TabPage("Roi", roiHistogramComponent);
			this.Pages.Add(_roiHistogramPage);

			PathProfileComponent pathProfileComponent = new PathProfileComponent(imageViewerToolContext);
			pathProfileComponent.Container = this;
			_pathProfilePage = new TabPage("Path", pathProfileComponent);
			this.Pages.Add(_pathProfilePage);
		}

		public override void Start()
		{
			base.Start();

			((RoiHistogramComponent)_roiHistogramPage.Component).Initialize();
			((PathProfileComponent)_pathProfilePage.Component).Initialize();
		}

		internal RoiAnalysisComponent SelectedComponent
		{
			get { return this.CurrentPage.Component as RoiAnalysisComponent; }
			set
			{
				if (value is RoiHistogramComponent)
					this.CurrentPage = _roiHistogramPage;
				else if (value is PathProfileComponent)
					this.CurrentPage = _pathProfilePage;
			}
		}

	}
}
