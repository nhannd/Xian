using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public abstract class SortPredicate : IComparer<StudyItem>
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

		public virtual int Compare(StudyItem x, StudyItem y)
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

		public override int Compare(StudyItem x, StudyItem y)
		{
			return base.Compare(y, x);
		}
	}
}