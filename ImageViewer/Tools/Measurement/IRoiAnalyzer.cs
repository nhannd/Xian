using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public enum RoiAnalysisMode
	{
		Dynamic = 0,

		Delayed
	}

	// NOTE: public setters on all properties 
	// allows for use of the IRoiAnalyzer for 
	// objects other than InteractiveGraphic, and
	// also allows them to be used outside the context
	// of the viewer (for example, if you have only the ImageSop
	// and you want to perform some roi calculations).

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
			_mode = RoiAnalysisMode.Dynamic;
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
			set { _imageRows = value; }
		}

		public int ImageColumns
		{
			get { return _imageColumns; }
			set { _imageColumns = value; }
		}

		public PixelData PixelData
		{
			get { return _pixelData; }
			set { _pixelData = value; }
		}

		public PixelSpacing NormalizedPixelSpacing
		{
			get { return _normalizedPixelSpacing; }
			set { _normalizedPixelSpacing = value; }
		}

		public IModalityLut ModalityLut
		{
			get { return _modalityLut; }
			set { _modalityLut = value; }
		}

		public RectangleF BoundingBox
		{
			get { return _boundingBox; }
			set { _boundingBox = value; }
		}

		public string Modality
		{
			get { return _modality; }
			set { _modality = value; }
		}

		public bool IsCTImage
		{
			get { return _modality == "CT"; }	
		}

		public virtual bool IsValid()
		{
			return this.PixelData != null;
		}

		/// <summary>
		/// Convenience method for initializing a <see cref="RoiInfo"/> object
		/// from an <see cref="InteractiveGraphic"/>.
		/// </summary>
		public virtual void Initialize(InteractiveGraphic roi)
		{
			IPresentationImage image = roi.ParentPresentationImage;
			IImageGraphicProvider provider = image as IImageGraphicProvider;
			if (provider == null)
				return;

			this.ImageRows = provider.ImageGraphic.Rows;
			this.ImageColumns = provider.ImageGraphic.Columns;

			this.PixelData = provider.ImageGraphic.PixelData;
			if (image is IModalityLutProvider)
				this.ModalityLut = ((IModalityLutProvider)image).ModalityLut;

			roi.CoordinateSystem = CoordinateSystem.Source;
			this.BoundingBox = roi.BoundingBox;
			roi.ResetCoordinateSystem();

			if (image is IImageSopProvider)
			{
				ImageSop sop = ((IImageSopProvider)image).ImageSop;
				this.NormalizedPixelSpacing = sop.NormalizedPixelSpacing;
				this.Modality = sop.Modality;
			}
			else
			{
				this.NormalizedPixelSpacing = new PixelSpacing(0, 0);
			}
		}
	}
	
	public interface IRoiAnalyzer<T> where T : RoiInfo
	{
		string Analyze(T roiInfo);
	}

	public sealed class RoiAnalyzerExtensionPoint<T> : ExtensionPoint<IRoiAnalyzer<T>> where T: RoiInfo
	{
		public RoiAnalyzerExtensionPoint()
		{
		}
	}
}
