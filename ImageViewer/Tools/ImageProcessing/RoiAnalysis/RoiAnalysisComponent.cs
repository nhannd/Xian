#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
