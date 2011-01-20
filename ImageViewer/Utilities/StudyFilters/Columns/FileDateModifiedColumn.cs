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
	[ExtensionOf(typeof (SpecialColumnExtensionPoint))]
	public class FileDateModifiedColumn : SpecialColumn<FileDateTime>, ITemporalSortableColumn
	{
		public const string KEY = "FileDateModified";

		public FileDateModifiedColumn() : base(SR.DateModified, KEY) {}

		public override FileDateTime GetTypedValue(IStudyItem item)
		{
			if (item == null || !File.Exists(item.Filename))
				return new FileDateTime(null);
			return new FileDateTime(File.GetLastWriteTime(item.Filename));
		}

		public override bool Parse(string input, out FileDateTime output)
		{
			return FileDateTime.TryParse(input, out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareTemporally(x, y);
		}

		public int CompareTemporally(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}