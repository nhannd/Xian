using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="ImageSetCollection"/> events.
	/// </summary>
	public class ImageSetEventArgs : CollectionEventArgs<IImageSet>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetEventArgs"/>.
		/// </summary>
		public ImageSetEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetEventArgs"/> with
		/// a specified <see cref="IImageSet"/>.
		/// </summary>
		/// <param name="imageSet"></param>
		public ImageSetEventArgs(IImageSet imageSet)
		{
			base.Item  = imageSet;
		}

		/// <summary>
		/// Gets the <see cref="IImageSet"/>.
		/// </summary>
		public IImageSet ImageSet { get { return base.Item; } }
	}
}
