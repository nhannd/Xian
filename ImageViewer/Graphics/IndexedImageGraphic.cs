using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image where pixel values are indices into a LUT.
	/// </summary>
	public abstract class IndexedImageGraphic : ImageGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <remarks>
		/// This constructor is provided for convenience in the case where
		/// the properties of <see cref="IndexedImageGraphic"/> are the
		/// same as that of an existing <see cref="ImageSop"/>.
		/// Note that a reference to <paramref name="imageSop"/> is <i>not</i> held
		/// by <see cref="IndexedImageGraphic"/>.
		/// </remarks>
		protected IndexedImageGraphic(ImageSop imageSop)
			: base(imageSop)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
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
		/// <param name="pixelData"></param>
		protected IndexedImageGraphic(
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
		/// The output of the LUT pipeline.
		/// </summary>
		/// <remarks>
		/// Each entry in the <see cref="OutputLUT"/> array is 32-bit ARGB value.
		/// When an <see cref="IRenderer"/> renders an <see cref="IndexedImageGraphic"/>, it should
		/// use <see cref="OutputLUT"/> to determine the ARGB value to display for a given pixel value.
		/// </remarks>
		public abstract int[] OutputLUT { get; }
	}
}
