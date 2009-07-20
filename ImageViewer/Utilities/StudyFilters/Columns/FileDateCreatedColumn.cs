using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class FileDateCreatedColumn : SpecialColumn<FileDateTime>, ITemporalSortableColumn
	{
		public const string KEY = "FileDateCreated";

		public FileDateCreatedColumn() : base(SR.DateCreated, KEY) { }

		public override FileDateTime GetTypedValue(StudyItem item)
		{
			if (item == null || !item.File.Exists)
				return new FileDateTime(null);
			return new FileDateTime(item.File.CreationTime);
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