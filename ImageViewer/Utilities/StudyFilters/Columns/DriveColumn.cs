using System.IO;
using System.Text.RegularExpressions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class DriveColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "Drive";

		public DriveColumn() : base(SR.Drive, KEY) { }

		public override string GetTypedValue(StudyItem item)
		{
			DriveInfo drive = GetDriveInfo(item);
			if (drive == null)
				return string.Empty;
			return drive.Name;
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

		internal static Regex _localDrive = new Regex("^[A-Za-z]:\\\\?$", RegexOptions.Compiled);

		internal static DriveInfo GetDriveInfo(StudyItem item)
		{
			if (item == null)
				return null;
			if (_localDrive.IsMatch(item.File.Directory.Root.FullName))
				return new DriveInfo(item.File.Directory.Root.FullName[0].ToString());
			return null;
		}
	}
}