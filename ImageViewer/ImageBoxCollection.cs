using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IImageBox"/> objects.
	/// </summary>
	public class ImageBoxCollection : ObservableList<IImageBox, ImageBoxEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageBoxCollection"/>.
		/// </summary>
		internal ImageBoxCollection()
		{

		}

		/// <summary>
		/// Creates a copy of the object.
		/// </summary>
		/// <param name="collection"></param>
		/// <remarks>
		/// Creates a <i>shallow</i> copy.  That is, only references to objects
		/// in the collection are copied.
		/// </remarks>
		public ImageBoxCollection(ObservableList<IImageBox, ImageBoxEventArgs> collection) 
			: base(collection)
		{
		}
	}
}
