using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiHistogram
{
	/// <summary>
	/// Extension point for views onto <see cref="RoiHistogramComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class RoiHistogramComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// RoiHistogramComponent class
	/// </summary>
	[AssociateView(typeof(RoiHistogramComponentViewExtensionPoint))]
	public class RoiHistogramComponent : ImageViewerToolComponent
	{
		private int _minBin = -200;
		private int _maxBin = 1000;
		private int _numBins = 100;
		private int[] _binLabels;
		private int[] _bins;
		private bool _autoscale;
		private bool _enabled = false;

		/// <summary>
		/// Constructor
		/// </summary>
		public RoiHistogramComponent(IImageViewerToolContext imageViewerToolContext)
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

		public int MinBin
		{
			get { return _minBin; }
			set 
			{ 
				_minBin = value;
				this.NotifyPropertyChanged("MinBin");
				OnSubjectChanged();
			}
		}

		public int MaxBin
		{
			get { return _maxBin; }
			set 
			{ 
				_maxBin = value;
				this.NotifyPropertyChanged("MaxBin");
				OnSubjectChanged();
			}
		}

		public int NumBins
		{
			get { return _numBins; }
			set 
			{ 
				_numBins = value;
				this.NotifyPropertyChanged("NumBins");
				OnSubjectChanged();
			}
		}

		public bool AutoScale
		{
			get { return _autoscale; }
			set 
			{ 
				_autoscale = value;
				this.NotifyPropertyChanged("AutoScale");
				OnSubjectChanged();
			}
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

		public int[] BinLabels
		{
			get { return _binLabels; }
		}

		public int[] Bins
		{
			get { return _bins; }
		}

		public bool ComputeHistogram()
		{
			RectangleInteractiveGraphic rectangle = GetSelectedRectangle();

			// For now, make sure the ROI is a rectangle
			if (rectangle == null)
			{
				this.Enabled = false;
				return false;
			}

			IImageGraphicProvider imageGraphicProvider =
				rectangle.ParentPresentationImage as IImageGraphicProvider;

			if (imageGraphicProvider == null)
			{
				this.Enabled = false;
				return false;
			}

			// For now, only allow ROIs of grayscale images
			GrayscaleImageGraphic image = imageGraphicProvider.ImageGraphic as GrayscaleImageGraphic;

			if (image == null)
			{
				this.Enabled = false;
				return false;
			}

			rectangle.CoordinateSystem = CoordinateSystem.Source;
			int left = (int)Math.Round(rectangle.Left);
			int right = (int)Math.Round(rectangle.Right);
			int top = (int)Math.Round(rectangle.Top);
			int bottom = (int)Math.Round(rectangle.Bottom);

			// If any part of the ROI is outside the bounds of the image,
			// don't allow a histogram to be displayed since it's invalid.
			if (left < 0 || left > image.Columns - 1 ||
				right < 0 || right > image.Columns - 1 ||
				top < 0 || top > image.Rows - 1 ||
				bottom < 0 || bottom > image.Rows - 1)
			{
				this.Enabled = false;
				return false;
			}

			int width = Math.Abs(right - left) + 1;
			int height = Math.Abs(bottom - top) + 1;
			rectangle.ResetCoordinateSystem();

			int[] roiPixelData = new int[width * height];

			image.PixelData.ForEachPixel(left, top, right, bottom,
				delegate(int i, int x, int y, int pixelIndex)
				{
					roiPixelData[i] = image.ModalityLUT[image.PixelData.GetPixel(pixelIndex)];
				});

			Histogram histogram = new Histogram(
				_minBin, _maxBin, _numBins, roiPixelData);

			_bins = histogram.Bins;
			_binLabels = histogram.BinLabels;

			NotifyPropertyChanged("MinBin");
			NotifyPropertyChanged("MaxBin");
			NotifyPropertyChanged("NumBins");

			this.Enabled = true;
			return true;
		}

		private RectangleInteractiveGraphic GetSelectedRectangle()
		{
			ROIGraphic graphic = GetSelectedRoi();

			if (graphic == null)
				return null;

			RectangleInteractiveGraphic rectangle = graphic.Roi as RectangleInteractiveGraphic;

			if (rectangle == null)
				return null;

			return rectangle;
		}

		private ROIGraphic GetSelectedRoi()
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
