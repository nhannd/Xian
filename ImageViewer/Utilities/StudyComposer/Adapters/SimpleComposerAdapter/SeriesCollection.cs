using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	public sealed class SeriesCollection : BindingListWrapper<SeriesItem>
	{
		internal SeriesCollection(BindingList<SeriesItem> list)
			: base(list) {}

		public SeriesItem GetFirstSeries()
		{
			if (this.Count == 0)
				return null;
			return this[0];
		}
	}
}