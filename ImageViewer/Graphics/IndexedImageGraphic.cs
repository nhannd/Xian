using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class IndexedImageGraphic : ImageGraphic
	{
		private LUTComposer _lutComposer;

		public IndexedImageGraphic(ImageSop imageSop) : base(imageSop)
		{
		}

		public IndexedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			int samplesPerPixel,
			int pixelRepresentation,
			int planarConfiguration,
			PhotometricInterpretation photometricInterpretation,
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
		}

		/// <summary>
		/// Gets the <see cref="LUTComposer"/> of the image.
		/// </summary>
		public LUTComposer LUTComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LUTComposer();

				return _lutComposer;
			}
		}
	}
}
