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
	public class FileSizeColumn : SpecialColumn<FileSize>, INumericSortableColumn
	{
		public const string KEY = "FileSize";

		public FileSizeColumn() : base(SR.FileSize, KEY) { }

		public override FileSize GetTypedValue(IStudyItem item)
		{
			if (item == null || !File.Exists(item.Filename))
				return new FileSize(-1);
			return new FileSize(new FileInfo(item.Filename).Length);
		}

		public override bool Parse(string input, out FileSize output)
		{
			return Columns.FileSize.TryParse(input, out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}