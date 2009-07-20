using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class FileSizeColumn : SpecialColumn<FileSize>, INumericSortableColumn
	{
		public const string KEY = "FileSize";

		public FileSizeColumn() : base(SR.FileSize, KEY) { }

		public override FileSize GetTypedValue(StudyItem item)
		{
			if (item == null || !item.File.Exists)
				return new FileSize(-1);
			return new FileSize(item.File.Length);
		}

		public override bool Parse(string input, out FileSize output)
		{
			return Columns.FileSize.TryParse(input, out output);
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(StudyItem x, StudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}