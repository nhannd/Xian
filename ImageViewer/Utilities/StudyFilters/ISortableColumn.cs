using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IGenericSortableColumn : IComparer<StudyItem> {}

	public interface INumericSortableColumn
	{
		int CompareNumerically(StudyItem x, StudyItem y);
	}

	public interface ILexicalSortableColumn
	{
		int CompareLexically(StudyItem x, StudyItem y);
	}

	public interface ITemporalSortableColumn
	{
		int CompareTemporally(StudyItem x, StudyItem y);
	}

	public interface ISpatialSortableColumn
	{
		int CompareSpatially(StudyItem x, StudyItem y);
	}
}