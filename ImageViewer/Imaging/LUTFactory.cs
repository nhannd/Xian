using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	// LUT flyweight factory
	internal sealed class LUTFactory
	{
		private List<ModalityLUTLinear> _modalityLUTs = new List<ModalityLUTLinear>();
		private List<PresentationLUT> _presentationLUTs = new List<PresentationLUT>();

		public LUTFactory()
		{

		}

		internal ModalityLUTLinear GetModalityLUTLinear(
			int bitsStored,
			int pixelRepresentation,
			double rescaleSlope,
			double rescaleIntercept)
		{
			foreach (ModalityLUTLinear lut in _modalityLUTs)
			{
				if (lut.BitsStored == bitsStored &&
					lut.PixelRepresentation == pixelRepresentation &&
					lut.RescaleSlope == rescaleSlope &&
					lut.RescaleIntercept == rescaleIntercept)
					return lut;
			}

			ModalityLUTLinear modalityLut = new ModalityLUTLinear(
				bitsStored, 
				pixelRepresentation, 
				rescaleSlope, 
				rescaleIntercept);

			_modalityLUTs.Add(modalityLut);

			return modalityLut;
		}

		internal PresentationLUT GetPresentationLUT(
			int minInputValue,
			int maxInputValue,
			PhotometricInterpretation photometricInterpretation)
		{
			foreach (PresentationLUT lut in _presentationLUTs)
			{
				if (lut.MaxInputValue == maxInputValue &&
					lut.MinInputValue == minInputValue &&
					lut.PhotometricInterpretation == photometricInterpretation)
					return lut;
			}

			PresentationLUT presentationLut = new PresentationLUT(
				minInputValue, 
				maxInputValue, 
				photometricInterpretation);

			_presentationLUTs.Add(presentationLut);

			return presentationLut;
		}
	}
}
