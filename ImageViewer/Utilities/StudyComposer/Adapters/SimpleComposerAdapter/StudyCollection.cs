using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	public sealed class StudyCollection : BindingListWrapper<StudyItem>
	{
		internal StudyCollection(BindingList<StudyItem> list)
			: base(list) {}

		public StudyItem GetFirstStudy()
		{
			if (this.Count == 0)
				return null;
			return this[0];
		}
	}
}