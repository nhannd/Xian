#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class DriveTypeColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "DriveType";

		public DriveTypeColumn() : base(SR.DriveType, KEY) { }

		public override string GetTypedValue(IStudyItem item)
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

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}