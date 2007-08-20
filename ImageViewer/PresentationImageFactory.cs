using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer
{
	public static class PresentationImageFactory
	{
		public static IPresentationImage Create(ImageSop imageSop)
		{
			if (imageSop.PhotometricInterpretation == PhotometricInterpretation.PaletteColor)
			{
				throw new Exception("Palette color images not yet supported");
			}
			else if (imageSop.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ||
			         imageSop.PhotometricInterpretation == PhotometricInterpretation.Monochrome2)
			{
				return new DicomGrayscalePresentationImage(imageSop);
			}
			else
			{
				return new DicomColorPresentationImage(imageSop);
			}
		}
	}
}
