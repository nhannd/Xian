using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class PathColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "Path";

		public PathColumn() : base(SR.Path, KEY) { }

		public override string GetTypedValue(StudyItem item)
		{
			if (item == null)
				return string.Empty;
			return item.File.FullName;
		}

		public override bool Parse(string input, out string output)
		{
			output = input;
			return true;
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(StudyItem x, StudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}