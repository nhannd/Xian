using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Measurement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class RoiInfo
	{
		private RoiAnalysisMode _mode;

		private int _imageRows;
		private int _imageColumns;
		private string _modality;
		private PixelData _pixelData;
		private PixelSpacing _normalizedPixelSpacing;
		private IModalityLut _modalityLut;
		private RectangleF _boundingBox;

		protected RoiInfo()
		{
		}

		/// <summary>
		/// Set by the <see cref="MeasurementTool{T}"/>.
		/// </summary>
		public RoiAnalysisMode Mode
		{
			get { return _mode; }
			internal set { _mode = value; }
		}

		public int ImageRows
		{
			get { return _imageRows; }
		}

		public int ImageColumns
		{
			get { return _imageColumns; }
		}

		public PixelData PixelData
		{
			get { return _pixelData; }
		}

		public PixelSpacing NormalizedPixelSpacing
		{
			get { return _normalizedPixelSpacing; }
		}

		public IModalityLut ModalityLut
		{
			get { return _modalityLut; }
		}

		public RectangleF BoundingBox
		{
			get { return _boundingBox; }
		}

		public string Modality
		{
			get { return _modality; }
		}

		public virtual bool IsValid()
		{
			return this.PixelData != null;
		}

		/// <summary>
		/// Convenience method for initializing a <see cref="RoiInfo"/> object
		/// from an <see cref="InteractiveGraphic"/>.
		/// </summary>
		protected internal virtual void Initialize(InteractiveGraphic roi)
		{
			IPresentationImage image = roi.ParentPresentationImage;
			IImageGraphicProvider provider = image as IImageGraphicProvider;
			if (provider == null)
				return;

			_imageRows = provider.ImageGraphic.Rows;
			_imageColumns = provider.ImageGraphic.Columns;

			_pixelData = provider.ImageGraphic.PixelData;
			if (image is IModalityLutProvider)
				_modalityLut = ((IModalityLutProvider)image).ModalityLut;

			roi.CoordinateSystem = CoordinateSystem.Source;
			_boundingBox = roi.BoundingBox;
			roi.ResetCoordinateSystem();

			if (image is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider)image).Frame;
				_normalizedPixelSpacing = frame.NormalizedPixelSpacing;
				_modality = frame.ParentImageSop.Modality;
			}
			else
			{
				_normalizedPixelSpacing = new PixelSpacing(0, 0);
			}
		}
	}
}