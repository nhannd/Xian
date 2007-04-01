using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A grayscale <see cref="IndexedImageGraphic"/>.
	/// </summary>
	public class GrayscaleImageGraphic : IndexedImageGraphic
	{
		private LUTComposer _lutComposer;
		private static LUTFactory _lutFactory = new LUTFactory();

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <remarks>
		/// This constructor is provided for convenience in the case where
		/// the properties of <see cref="GrayscaleImageGraphic"/> are the
		/// same as that of an existing <see cref="ImageSop"/>.
		/// Note that a reference to <paramref name="imageSop"/> is <i>not</i> held
		/// by <see cref="GrayscaleImageGraphic"/>.
		/// </remarks>
		public GrayscaleImageGraphic(ImageSop imageSop) 
			: this(
			imageSop.Rows,
			imageSop.Columns,
			imageSop.BitsAllocated,
			imageSop.BitsStored,
			imageSop.HighBit,
			imageSop.SamplesPerPixel,
			imageSop.PixelRepresentation,
			imageSop.PlanarConfiguration,
			imageSop.PhotometricInterpretation,
			imageSop.RescaleSlope,
			imageSop.RescaleIntercept,
			imageSop.PixelData)
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="samplesPerPixel"></param>
		/// <param name="pixelRepresentation"></param>
		/// <param name="planarConfiguration"></param>
		/// <param name="photometricInterpretation"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelData"></param>
		public GrayscaleImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			int samplesPerPixel,
			int pixelRepresentation,
			int planarConfiguration,
			PhotometricInterpretation photometricInterpretation,
			double rescaleSlope,
			double rescaleIntercept,
			byte[] pixelData)
			: base(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				samplesPerPixel,
				pixelRepresentation,
				planarConfiguration,
				photometricInterpretation,
				pixelData)
		{
			InstallGrayscaleLUTs(rescaleSlope, rescaleIntercept, photometricInterpretation);
		}

		/// <summary>
		/// Gets the modality LUT.
		/// </summary>
		public IComposableLUT ModalityLUT
		{
			get { return this.LUTComposer.LUTCollection[0]; }
		}

		/// <summary>
		/// Gets the VOI LUT.
		/// </summary>
		public IComposableLUT VoiLUT
		{
			get { return this.LUTComposer.LUTCollection[1]; }
		}

		/// <summary>
		/// Gets the linear VOI LUT.
		/// </summary>
		/// <value>The linear VOI LUT or <b>null</b> if the VOI LUT
		/// is not linear.</value>
		public IVOILUTLinear VoiLUTLinear
		{
			get { return this.VoiLUT as IVOILUTLinear; }
		}

		/// <summary>
		/// Gets the presentation LUT.
		/// </summary>
		public IComposableLUT PresentationLUT
		{
			get { return this.LUTComposer.LUTCollection[2]; }
		}

		public override int[] OutputLUT
		{
			get { return this.LUTComposer.OutputLUT; }
		}

		/// <summary>
		/// Gets the <see cref="LUTComposer"/>.
		/// </summary>
		protected LUTComposer LUTComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LUTComposer();

				return _lutComposer;
			}
		}

		private void InstallGrayscaleLUTs(
			double rescaleSlope, 
			double rescaleIntercept,
			PhotometricInterpretation photometricInterpretation)
		{
			ModalityLUTLinear modalityLut = _lutFactory.GetModalityLUTLinear(
				this.BitsStored,
				this.PixelRepresentation,
				rescaleSlope,
				rescaleIntercept);

			this.LUTComposer.LUTCollection.Add(modalityLut);

			VOILUTLinear voiLut = new VOILUTLinear(
				modalityLut.MinOutputValue,
				modalityLut.MaxOutputValue);

			this.LUTComposer.LUTCollection.Add(voiLut);

			PresentationLUT presentationLut = _lutFactory.GetPresentationLUT(
				voiLut.MinOutputValue,
				voiLut.MaxOutputValue,
				photometricInterpretation);

			this.LUTComposer.LUTCollection.Add(presentationLut);
		}
	}
}
