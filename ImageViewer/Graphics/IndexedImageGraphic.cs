using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image where pixel values are indices into a LUT.
	/// </summary>
	public abstract class IndexedImageGraphic : ImageGraphic, IIndexedPixelDataProvider
	{
		#region Private fields

		private int _bitsStored;
		private int _highBit;
		private bool _isSigned;
		private bool _invert;

		#endregion

		#region Protected constructors

		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <remarks>
		/// Creates an empty indexed image of a specific size and bit depth.
		/// All all entries in the byte buffer are set to zero. Useful as
		/// a canvas on which pixels can be set by the client.
		/// </remarks>
		protected IndexedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool invert)
			: base(
				rows,
				columns,
				bitsAllocated)
		{
			Initialize(bitsStored, highBit, isSigned, invert);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <remarks>
		/// Creates an indexed image using existing pixel data.
		/// </remarks>
		protected IndexedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool invert,
			byte[] pixelData)
			: base(
				rows,
				columns,
				bitsAllocated,
				pixelData)
		{
			Initialize(bitsStored, highBit, isSigned, invert);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated">Can be either 8 or 16.</param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates an indexed image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		protected IndexedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool invert,
			PixelDataGetter pixelDataGetter)
			: base(
				rows,
				columns,
				bitsAllocated,
				pixelDataGetter)
		{
			Initialize(bitsStored, highBit, isSigned, invert);
		}

		private void Initialize(int bitsStored, int highBit, bool isSigned, bool invert)
		{
			DicomValidator.ValidateBitsStored(bitsStored);
			DicomValidator.ValidateHighBit(highBit);

			_bitsStored = bitsStored;
			_highBit = highBit;
			_isSigned = isSigned;
			_invert = Invert;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets the number of bits stored in the image.
		/// </summary>
		/// <remarks>The number of bits stored does not necessarily equal the number
		/// of bits allocated. Values of 8, 10, 12 and 16 are typical.</remarks>
		public int BitsStored
		{
			get { return _bitsStored; }
		}

		/// <summary>
		/// Gets the high bit in the image.
		/// </summary>
		/// <remarks>
		/// Theoretically, the high bit does not necessarily have to equal
		/// Bits Stored - 1.  But in almost all cases this assumption is true; we
		/// too make this assumption.
		/// </remarks>
		public int HighBit
		{
			get { return _highBit; }
		}

		/// <summary>
		/// Gets a value indicating whether the image's pixel data is signed.
		/// </summary>
		public bool IsSigned
		{
			get { return _isSigned; }
		}

		public bool Invert
		{
			get { return _invert; }
			set { _invert = value; }
		}

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public new IndexedPixelData PixelData
		{
			get
			{
				return base.PixelData as IndexedPixelData;
			}
		}

		/// <summary>
		/// The output of the LUT pipeline.
		/// </summary>
		public abstract IComposedLut OutputLut { get; }

		/// <summary>
		/// The color map for the image, if applicable.
		/// </summary>
		/// <remarks>
		/// Each entry in the <see cref="ColorMap"/> array is 32-bit ARGB value.
		/// When an <see cref="IRenderer"/> renders an <see cref="IndexedImageGraphic"/>, it should
		/// use <see cref="ColorMap"/> to determine the ARGB value to display for a given pixel value.
		/// </remarks>
		public abstract IColorMap ColorMap { get; }

		#endregion

		/// <summary>
		/// Creates an object that encapsulates the pixel data.
		/// </summary>
		/// <returns></returns>
		protected override PixelData CreatePixelDataWrapper()
		{
			if (this.PixelDataRaw != null)
			{
				return new IndexedPixelData(
									this.Rows,
									this.Columns,
									this.BitsPerPixel,
									this.BitsStored,
									this.HighBit,
									this.IsSigned,
									this.PixelDataRaw);
			}
			else
			{
				return new IndexedPixelData(
									this.Rows,
									this.Columns,
									this.BitsPerPixel,
									this.BitsStored,
									this.HighBit,
									this.IsSigned,
									this.PixelDataGetter);
			}
		}
	}
}
