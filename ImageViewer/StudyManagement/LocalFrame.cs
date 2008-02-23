using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A local, file-based implementation of <see cref="Frame"/>.
	/// </summary>
	public class LocalFrame : Frame
	{
		private byte[] _pixelData;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalFrame"/>.
		/// </summary>
		/// <param name="parentImageSop"></param>
		/// <param name="frameNumber"></param>
		protected internal LocalFrame(LocalImageSop parentImageSop, int frameNumber)
			: base(parentImageSop, frameNumber)
		{

		}

		private LocalImageSop ParentLocalImageSop
		{
			get { return this.ParentImageSop as LocalImageSop; }
		}


		/// <summary>
		/// Gets pixel data in normalized form.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <i>Normalized</i> pixel data means that:
		/// <list type="Bullet">
		/// <item>
		/// <description>Grayscale pixel data is unchanged.</description>
		/// </item>
		/// <item>
		/// <description>Colour pixel data is always converted
		/// into ARGB format.</description>
		/// </item>
		/// <item>
		/// <description>Pixel data is always uncompressed.</description>
		/// </item>
		/// </list>
		/// Ensuring that the pixel data always meets the above criteria
		/// allows clients to easily consume pixel data without having
		/// to worry about the the multitude of DICOM photometric interpretations
		/// and transfer syntaxes.
		/// </remarks>
		/// <seealso cref="Frame.NormalizePixelData"/>
		/// <seealso cref="Frame.DecompressPixelData"/>
		public override byte[] GetNormalizedPixelData()
		{
			this.ParentLocalImageSop.Load();

			if (_pixelData == null)
			{
				DicomUncompressedPixelData pixelData = new DicomUncompressedPixelData(this.ParentLocalImageSop.NativeDicomObject.DataSet);
				_pixelData = NormalizePixelData(pixelData.GetFrame(this.FrameNumber));
			}

			return _pixelData;
		}
	}
}
