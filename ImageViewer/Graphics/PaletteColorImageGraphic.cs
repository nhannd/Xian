using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A palette colour <see cref="IndexedImageGraphic"/>.
	/// </summary>
	public class PaletteColorImageGraphic
		: IndexedImageGraphic
	{
		private readonly ImageSop _imageSop;
		private IColorMap _colorMap;

		// Normally, we don't associate the ImageSop directly with an ImageGraphic,
		// since we don't want ImageGraphics to be coupled to any DICOM concepts.  
		// However, there's nothing about PaletteColor images that can't be achieved
		// through simply using a GrayscaleImage with a ColorMap, so why not just make
		// our lives easier and associate the ImageSop with the PaletteColorImageGraphic.

		/// <summary>
		/// Initializes a new instance of <see cref="PaletteColorImageGraphic"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		public PaletteColorImageGraphic(ImageSop imageSop)
			: base(
				imageSop.Rows,
				imageSop.Columns,
				imageSop.BitsAllocated,
				imageSop.BitsStored,
				imageSop.HighBit,
				imageSop.PixelRepresentation != 0 ? true : false,
				imageSop.GetNormalizedPixelData)
		{
			_imageSop = imageSop;
		}

		/// <summary>
		/// The colour map of the image.
		/// </summary>
		/// <remarks>
		/// This colour map is constructed from the palette colour lookup table data
		/// encoded in the DICOM header.
		/// </remarks>
		public override IColorMap ColorMap
		{
			get
			{
				if (_colorMap == null)
					CreateColorMap();

				return _colorMap;
			}
		}

		private void CreateColorMap()
		{
			bool tagExists;
			int lutSize, firstMappedPixel, bitsPerLutEntry;

			_imageSop.GetTag(DicomTags.RedPaletteColorLookupTableDescriptor, out lutSize, 0, out tagExists);

			if (!tagExists)
				throw new Exception("LUT Size missing.");

			_imageSop.GetTag(DicomTags.RedPaletteColorLookupTableDescriptor, out firstMappedPixel, 1, out tagExists);

			if (!tagExists)
				throw new Exception("First Mapped Pixel missing.");
			
			_imageSop.GetTag(DicomTags.RedPaletteColorLookupTableDescriptor, out bitsPerLutEntry, 2, out tagExists);

			if (!tagExists)
				throw new Exception("Bits Per Entry missing.");

			byte[] redLut, greenLut, blueLut;

			_imageSop.GetTagOBOW(DicomTags.RedPaletteColorLookupTableData, out redLut, out tagExists);

			if (!tagExists)
				throw new Exception("Red Palette Color LUT missing.");

			_imageSop.GetTagOBOW(DicomTags.GreenPaletteColorLookupTableData, out greenLut, out tagExists);

			if (!tagExists)
				throw new Exception("Green Palette Color LUT missing.");
			
			_imageSop.GetTagOBOW(DicomTags.BluePaletteColorLookupTableData, out blueLut, out tagExists);
			
			if (!tagExists)
				throw new Exception("Blue Palette Color LUT missing.");

			// The DICOM standard says that if the LUT size is 0, it means that it's 65536 in size.
			if (lutSize == 0)
				lutSize = 65536;

			_colorMap = new PaletteColorMap(
				lutSize, 
				firstMappedPixel, 
				bitsPerLutEntry,
				redLut,
				greenLut,
				blueLut);
		}
	}
}
