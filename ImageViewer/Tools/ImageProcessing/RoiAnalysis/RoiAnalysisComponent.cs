using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	public abstract class RoiAnalysisComponent : ImageViewerToolComponent
	{
		private bool _enabled = false;
		private RoiAnalysisComponentContainer _container;

		protected RoiAnalysisComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext)
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

		public ROIGraphic GetSelectedRoi()
		{
			if (this.ImageViewer == null)
				return null;

			if (this.ImageViewer.SelectedPresentationImage == null)
				return null;

			if (this.ImageViewer.SelectedPresentationImage.SelectedGraphic == null)
				return null;

			ROIGraphic graphic =
				this.ImageViewer.SelectedPresentationImage.SelectedGraphic as ROIGraphic;

			return graphic;
		}

		protected abstract bool CanAnalyzeSelectedRoi();

		protected override void OnSubjectChanged()
		{
			if (CanAnalyzeSelectedRoi())
			{
				if (this.Container != null)
					this.Container.SelectedComponent = this;
			}

			base.OnSubjectChanged();
		}

		internal void Initialize()
		{
			OnSubjectChanged();
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			if (e.DeactivatedImageViewer != null)
				e.DeactivatedImageViewer.EventBroker.GraphicSelectionChanged -= new EventHandler<GraphicSelectionChangedEventArgs>(OnGraphicSelectionChanged);

			if (e.ActivatedImageViewer != null)
				e.ActivatedImageViewer.EventBroker.GraphicSelectionChanged += new EventHandler<GraphicSelectionChangedEventArgs>(OnGraphicSelectionChanged);

			OnSubjectChanged();
		}

		void OnGraphicSelectionChanged(object sender, GraphicSelectionChangedEventArgs e)
		{
			ROIGraphic deselectedGraphic = e.DeselectedGraphic as ROIGraphic;
			ROIGraphic selectedGraphic = e.SelectedGraphic as ROIGraphic;

			UnwatchRoiGraphic(deselectedGraphic);
			WatchRoiGraphic(selectedGraphic);

			OnSubjectChanged();
		}

		private void UnwatchRoiGraphic(ROIGraphic roiGraphic)
		{
			if (roiGraphic != null)
				roiGraphic.RoiChanged -= OnRoiChanged;
		}

		private void WatchRoiGraphic(ROIGraphic roiGraphic)
		{
			if (roiGraphic != null)
				roiGraphic.RoiChanged += OnRoiChanged;
		}

		private void OnRoiChanged(object sender, EventArgs e)
		{
			OnSubjectChanged();
		}
	}
}
