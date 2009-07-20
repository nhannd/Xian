using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class FileDateAccessedColumn : SpecialColumn<FileDateTime>, ITemporalSortableColumn
	{
		public const string KEY = "FileDateAccessed";

		public FileDateAccessedColumn() : base(SR.DateAccessed, KEY) { }

		public override FileDateTime GetTypedValue(StudyItem item)
		{
			if (item == null || !item.File.Exists)
				return new FileDateTime(null);
			return new FileDateTime(item.File.LastAccessTime);
		}

		public override bool Parse(string input, out FileDateTime output)
		{
			return FileDateTime.TryParse(input, out output);
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareTemporally(x, y);
		}

		public int CompareTemporally(StudyItem x, StudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}