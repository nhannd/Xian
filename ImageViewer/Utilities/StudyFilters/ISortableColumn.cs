#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IGenericSortableColumn : IComparer<IStudyItem> {}

	public interface INumericSortableColumn
	{
		int CompareNumerically(IStudyItem x, IStudyItem y);
	}

	public interface ILexicalSortableColumn
	{
		int CompareLexically(IStudyItem x, IStudyItem y);
	}

	public interface ITemporalSortableColumn
	{
		int CompareTemporally(IStudyItem x, IStudyItem y);
	}

	public interface ISpatialSortableColumn
	{
		int CompareSpatially(IStudyItem x, IStudyItem y);
	}
}