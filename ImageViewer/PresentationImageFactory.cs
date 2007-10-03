using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A static factory class which creates <see cref="IPresentationImage"/>s.
	/// </summary>
	public static class PresentationImageFactory
	{
		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// based on the <see cref="ImageSop"/>'s photometric interpretation.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <returns></returns>
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
