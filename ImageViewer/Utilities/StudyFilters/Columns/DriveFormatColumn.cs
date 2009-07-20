using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class DriveFormatColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "DriveFormat";

		public DriveFormatColumn() : base(SR.DriveFormat, KEY) { }

		public override string GetTypedValue(StudyItem item)
		{
			DriveInfo drive = DriveColumn.GetDriveInfo(item);
			if (drive == null)
				return string.Empty;
			return drive.DriveFormat;
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