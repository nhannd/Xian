using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class GrayscaleImageGraphic : IndexedImageGraphic
	{
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

		public IComposableLUT ModalityLUT
		{
			get { return this.LUTComposer.LUTCollection[0]; }
		}

		public IComposableLUT VoiLUT
		{
			get { return this.LUTComposer.LUTCollection[1]; }
		}

		public IVOILUTLinear VoiLUTLinear
		{
			get { return this.VoiLUT as IVOILUTLinear; }
		}

		public IComposableLUT PresentationLUT
		{
			get { return this.LUTComposer.LUTCollection[2]; }
		}

		private void InstallGrayscaleLUTs(
			double rescaleSlope, 
			double rescaleIntercept,
			PhotometricInterpretation photometricInterpretation)
		{
			ModalityLUTLinear modalityLut = new ModalityLUTLinear(
				this.BitsStored,
				this.PixelRepresentation,
				rescaleSlope,
				rescaleIntercept);

			this.LUTComposer.LUTCollection.Add(modalityLut);

			VOILUTLinear voiLut = new VOILUTLinear(
				modalityLut.MinOutputValue,
				modalityLut.MaxOutputValue);

			this.LUTComposer.LUTCollection.Add(voiLut);

			PresentationLUT presentationLut = new PresentationLUT(
				voiLut.MinOutputValue,
				voiLut.MaxOutputValue,
				photometricInterpretation);

			this.LUTComposer.LUTCollection.Add(presentationLut);
		}
	}
}
