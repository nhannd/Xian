using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	public sealed class ImageCollection : BindingListWrapper<ImageItem>
	{
		internal ImageCollection(BindingList<ImageItem> list)
			: base(list) {}

		public ImageItem GetFirstImage()
		{
			if (this.Count == 0)
				return null;
			return this[0];
		}
	}
}