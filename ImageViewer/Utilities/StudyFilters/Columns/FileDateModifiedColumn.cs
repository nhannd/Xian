using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof (SpecialColumnExtensionPoint))]
	public class FileDateModifiedColumn : SpecialColumn<FileDateTime>, ITemporalSortableColumn
	{
		public const string KEY = "FileDateModified";

		public FileDateModifiedColumn() : base(SR.DateModified, KEY) {}

		public override FileDateTime GetTypedValue(StudyItem item)
		{
			if (item == null || !item.File.Exists)
				return new FileDateTime(null);
			return new FileDateTime(item.File.LastWriteTime);
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