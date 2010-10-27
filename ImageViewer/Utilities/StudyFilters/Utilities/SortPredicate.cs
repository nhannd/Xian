#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public abstract class SortPredicate : IComparer<IStudyItem>
	{
		public readonly StudyFilterColumn Column;

		public SortPredicate(StudyFilterColumn column)
		{
			this.Column = column;
		}

		public override sealed bool Equals(object obj)
		{
			SortPredicate other = obj as SortPredicate;
			if (other != null)
				return this.Column == other.Column && this.GetType() == other.GetType();
			return false;
		}

		public override sealed int GetHashCode()
		{
			return 0x00DBFF0B ^ this.GetType().GetHashCode() ^ this.Column.GetHashCode();
		}

		public virtual int Compare(IStudyItem x, IStudyItem y)
		{
			return this.Column.Compare(x, y);
		}
	}

	public sealed class AscendingSortPredicate : SortPredicate
	{
		public AscendingSortPredicate(StudyFilterColumn column) : base(column) {}
	}

	public sealed class DescendingSortPredicate : SortPredicate
	{
		public DescendingSortPredicate(StudyFilterColumn column) : base(column) {}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return base.Compare(y, x);
		}
	}
}