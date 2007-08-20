using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;

/*
namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="GrayscaleImageGraphic"/> with an associated <see cref="ImageSop"/>.
	/// </summary>
	public class StandardGrayscaleImageGraphic : GrayscaleImageGraphic, IVoiLutManagerProvider, IAutoVoiLutApplicatorProvider
	{
		#region Private fields

		private ImageSop _imageSop;
		private IVoiLutManager _voiLutManager;

		#endregion

		#region Public constructor

		public StandardGrayscaleImageGraphic(ImageSop imageSop, int rows, int columns)
			: base(rows, columns)
		{
			Platform.CheckForNullReference(imageSop, "image");
			_imageSop = imageSop;

			_voiLutManager = CreateVoiLutManager();
			Platform.CheckForNullReference(_voiLutManager, "_voiLutManager");

			if (imageSop.PhotometricInterpretation != PhotometricInterpretation.Monochrome1 &&
				imageSop.PhotometricInterpretation != PhotometricInterpretation.Monochrome2)
				throw new InvalidOperationException("Image must be MONOCHROME1 or MONOCHROME2.");

			ApplyInitialVoiLut();
		}

		/// <summary>
		/// Instantiates a new instance of <see cref="StandardGrayscaleImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		public StandardGrayscaleImageGraphic(ImageSop imageSop) : 
			base(
			imageSop.Rows,
			imageSop.Columns,
			imageSop.BitsAllocated,
			imageSop.BitsStored,
			imageSop.HighBit,
			imageSop.PixelRepresentation != 0 ? true : false,
			imageSop.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ? true: false,
			imageSop.RescaleSlope,
			imageSop.RescaleIntercept,
			new PixelDataGetter(imageSop.GetNormalizedPixelData))
		{
			Platform.CheckForNullReference(imageSop, "image");
			_imageSop = imageSop;

			_voiLutManager = CreateVoiLutManager();
			Platform.CheckForNullReference(_voiLutManager, "_voiLutManager");

			if (imageSop.PhotometricInterpretation != PhotometricInterpretation.Monochrome1 &&
				imageSop.PhotometricInterpretation != PhotometricInterpretation.Monochrome2)
				throw new InvalidOperationException("Image must be MONOCHROME1 or MONOCHROME2.");

			ApplyInitialVoiLut();
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets the associated <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		/// <summary>	
		/// Gets the Window Width/Center values from the Dicom Header.
		/// </summary>
		public Window[] WindowCenterValues
		{
			get { return ImageSop.WindowCenterAndWidth; }
		}

		/// <summary>
		/// Returns the number of Linear Header Luts in the Dicom Header.
		/// </summary>
		public int NumberOfWindowCenterValues
		{
			get { return this.AnyWindowCenterValues ? this.WindowCenterValues.Length : 0; }
		}

		/// <summary>
		/// Returns whether or not there are any Linear Header Luts.
		/// </summary>
		public bool AnyWindowCenterValues
		{
			get { return !(this.WindowCenterValues == null || this.WindowCenterValues.Length == 0); }
		}

		#region IAutoLutApplicatorProvider Members

		public virtual IAutoVoiLutApplicator AutoVoiLutApplicator
		{
			get { return _voiLutManager as IAutoVoiLutApplicator; }
		}

		#endregion

		#region IVoiLutManagerProvider Members

		public IVoiLutManager VoiLutManager
		{
			get { return _voiLutManager; }
		}

		#endregion

		#endregion

		#region Public methods

		/// <summary>
		/// Installs a new Voi Lut
		/// </summary>
		/// <param name="newLut">the Lut to install</param>
		public virtual void InstallVoiLut(IComposableLUT newLut)
		{
			Platform.CheckForNullReference(newLut, "newLut");
			base.VoiLUT = newLut;
		}

		#endregion

		#region Protected methods

		protected virtual void ApplyInitialVoiLut()
		{
			if (this.AutoVoiLutApplicator != null)
				this.AutoVoiLutApplicator.ApplyNext();
		}

		protected virtual IVoiLutManager CreateVoiLutManager()
		{
			return new StandardVoiLutManager(this);
		}

		#endregion
	}
}
*/