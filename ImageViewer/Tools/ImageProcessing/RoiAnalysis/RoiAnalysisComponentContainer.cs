#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
