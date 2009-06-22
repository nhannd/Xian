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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	public abstract class RoiAnalysisComponent : ImageViewerToolComponent
	{
		private bool _enabled = false;
		private RoiAnalysisComponentContainer _container;

		protected RoiAnalysisComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext.DesktopWindow)
		{
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				NotifyPropertyChanged("Enabled");
			}
		}

		internal RoiAnalysisComponentContainer Container
		{
			get { return _container; }
			set { _container = value; }
		}

		public override void Start()
		{
			// If there's an ROI selected already when 
			WatchRoiGraphic(GetSelectedRoi());

			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public RoiGraphic GetSelectedRoi()
		{
			if (this.ImageViewer == null)
				return null;

			if (this.ImageViewer.SelectedPresentationImage == null)
				return null;

			if (this.ImageViewer.SelectedPresentationImage.SelectedGraphic == null)
				return null;

			RoiGraphic graphic =
				this.ImageViewer.SelectedPresentationImage.SelectedGraphic as RoiGraphic;

			return graphic;
		}

		protected abstract bool CanAnalyzeSelectedRoi();

		protected void OnAllPropertiesChanged()
		{
			if (CanAnalyzeSelectedRoi())
			{
				if (this.Container != null)
					this.Container.SelectedComponent = this;
			}

			base.NotifyAllPropertiesChanged();
		}

		internal void Initialize()
		{
			OnAllPropertiesChanged();
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			if (e.DeactivatedImageViewer != null)
				e.DeactivatedImageViewer.EventBroker.GraphicSelectionChanged -= new EventHandler<GraphicSelectionChangedEventArgs>(OnGraphicSelectionChanged);

			if (e.ActivatedImageViewer != null)
				e.ActivatedImageViewer.EventBroker.GraphicSelectionChanged += new EventHandler<GraphicSelectionChangedEventArgs>(OnGraphicSelectionChanged);

			OnAllPropertiesChanged();
		}

		private void OnGraphicSelectionChanged(object sender, GraphicSelectionChangedEventArgs e)
		{
			RoiGraphic deselectedGraphic = e.DeselectedGraphic as RoiGraphic;
			RoiGraphic selectedGraphic = e.SelectedGraphic as RoiGraphic;

			UnwatchRoiGraphic(deselectedGraphic);
			WatchRoiGraphic(selectedGraphic);

			OnAllPropertiesChanged();
		}

		private void UnwatchRoiGraphic(RoiGraphic roiGraphic)
		{
			if (roiGraphic != null)
				roiGraphic.RoiChanged -= OnRoiChanged;
		}

		private void WatchRoiGraphic(RoiGraphic roiGraphic)
		{
			if (roiGraphic != null)
				roiGraphic.RoiChanged += OnRoiChanged;
		}

		private void OnRoiChanged(object sender, EventArgs e)
		{
			OnAllPropertiesChanged();
		}
	}
}
