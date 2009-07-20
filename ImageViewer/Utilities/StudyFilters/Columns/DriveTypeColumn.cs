using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class DriveTypeColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "DriveType";

		public DriveTypeColumn() : base(SR.DriveType, KEY) { }

		public override string GetTypedValue(StudyItem item)
		{
			DriveInfo drive = DriveColumn.GetDriveInfo(item);
			if (drive != null)
			{
				switch (drive.DriveType)
				{
					case DriveType.Removable:
						return SR.RemovableMedia;
					case DriveType.Fixed:
						return SR.FixedMedia;
					case DriveType.Network:
						return SR.NetworkMedia;
					case DriveType.CDRom:
						return SR.CDRomMedia;
					case DriveType.Ram:
						return SR.RAM;
				}
			}
			return SR.Unknown;
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