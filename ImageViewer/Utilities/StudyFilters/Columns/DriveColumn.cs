#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

		public override string GetTypedValue(IStudyItem item)
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

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}

		private static readonly Regex _localDrive = new Regex("^[A-Za-z]:\\\\?$", RegexOptions.Compiled);

		internal static DriveInfo GetDriveInfo(IStudyItem item)
		{
			if (item == null)
				return null;
			string root = Path.GetPathRoot(item.Filename);
			if (_localDrive.IsMatch(root))
				return new DriveInfo(root[0].ToString());
			return null;
		}
	}
}