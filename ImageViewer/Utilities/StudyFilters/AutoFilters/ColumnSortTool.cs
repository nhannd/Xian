#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters
{
	[MenuAction("sortUp", "studyfilters-columnfilters/MenuSortAscending", "SortAscending")]
	[IconSet("sortUp", IconScheme.Colour, "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png")]
	[VisibleStateObserver("sortUp", "Visible", "VisibleChanged")]
	[MenuAction("sortDown", "studyfilters-columnfilters/MenuSortDescending", "SortDescending")]
	[IconSet("sortDown", IconScheme.Colour, "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png")]
	[VisibleStateObserver("sortDown", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class ColumnSortTool : AutoFilterTool
	{
		protected override bool IsColumnSupported()
		{
			return base.Column is IGenericSortableColumn;
		}

		public void RemoveSort()
		{
			SortPredicate existingPredicate = null;
			foreach (SortPredicate predicate in this.Context.Column.Owner.SortPredicates)
			{
				if (predicate.Column.Key == this.Context.Column.Key)
					existingPredicate = predicate;
			}
			if (existingPredicate != null)
				this.Context.Column.Owner.SortPredicates.Remove(existingPredicate);
		}

		public void SortAscending()
		{
			this.RemoveSort();
			this.StudyFilter.SortPredicates.Insert(0, this.CreateAscendingSortPredicate());
			this.StudyFilter.Refresh(true);
		}

		public void SortDescending()
		{
			this.RemoveSort();
			this.StudyFilter.SortPredicates.Insert(0, this.CreateDescendingSortPredicate());
			this.StudyFilter.Refresh(true);
		}

		protected virtual SortPredicate CreateAscendingSortPredicate()
		{
			return new AscendingSortPredicate(this.Column);
		}

		protected virtual SortPredicate CreateDescendingSortPredicate()
		{
			return new DescendingSortPredicate(this.Column);
		}
	}

	[MenuAction("sortUp", "studyfilters-columnfilters/MenuSortAscendingLexically", "SortAscending")]
	[IconSet("sortUp", IconScheme.Colour, "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png")]
	[VisibleStateObserver("sortUp", "Visible", "VisibleChanged")]
	[MenuAction("sortDown", "studyfilters-columnfilters/MenuSortDescendingLexically", "SortDescending")]
	[IconSet("sortDown", IconScheme.Colour, "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png")]
	[VisibleStateObserver("sortDown", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class LexicalColumnSortTool : ColumnSortTool
	{
		protected override bool IsColumnSupported()
		{
			return base.Column is ILexicalSortableColumn;
		}

		protected override SortPredicate CreateAscendingSortPredicate()
		{
			return new LexicalSortPredicate(base.Column, false);
		}

		protected override SortPredicate CreateDescendingSortPredicate()
		{
			return new LexicalSortPredicate(base.Column, true);
		}

		private class LexicalSortPredicate : SortPredicate
		{
			public readonly bool Descending;

			public LexicalSortPredicate(StudyFilterColumn column, bool descending) : base(column)
			{
				this.Descending = descending;
			}

			public override int Compare(StudyItem x, StudyItem y)
			{
				if (this.Descending)
				{
					StudyItem z = y;
					y = x;
					x = z;
				}
				return ((ILexicalSortableColumn) base.Column).CompareLexically(x, y);
			}
		}
	}

	[MenuAction("sortUp", "studyfilters-columnfilters/MenuSortAscendingTemporally", "SortAscending")]
	[IconSet("sortUp", IconScheme.Colour, "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png")]
	[VisibleStateObserver("sortUp", "Visible", "VisibleChanged")]
	[MenuAction("sortDown", "studyfilters-columnfilters/MenuSortDescendingTemporally", "SortDescending")]
	[IconSet("sortDown", IconScheme.Colour, "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png")]
	[VisibleStateObserver("sortDown", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class TemporalColumnSortTool : ColumnSortTool
	{
		protected override bool IsColumnSupported()
		{
			return base.Column is ITemporalSortableColumn;
		}

		protected override SortPredicate CreateAscendingSortPredicate()
		{
			return new TemporalSortPredicate(base.Column, false);
		}

		protected override SortPredicate CreateDescendingSortPredicate()
		{
			return new TemporalSortPredicate(base.Column, true);
		}

		private class TemporalSortPredicate : SortPredicate
		{
			public readonly bool Descending;

			public TemporalSortPredicate(StudyFilterColumn column, bool descending)
				: base(column)
			{
				this.Descending = descending;
			}

			public override int Compare(StudyItem x, StudyItem y)
			{
				if (this.Descending)
				{
					StudyItem z = y;
					y = x;
					x = z;
				}
				return ((ITemporalSortableColumn) base.Column).CompareTemporally(x, y);
			}
		}
	}

	[MenuAction("sortUp", "studyfilters-columnfilters/MenuSortAscendingNumerically", "SortAscending")]
	[IconSet("sortUp", IconScheme.Colour, "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png")]
	[VisibleStateObserver("sortUp", "Visible", "VisibleChanged")]
	[MenuAction("sortDown", "studyfilters-columnfilters/MenuSortDescendingNumerically", "SortDescending")]
	[IconSet("sortDown", IconScheme.Colour, "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png")]
	[VisibleStateObserver("sortDown", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class NumericalColumnSortTool : ColumnSortTool
	{
		protected override bool IsColumnSupported()
		{
			return base.Column is INumericSortableColumn;
		}

		protected override SortPredicate CreateAscendingSortPredicate()
		{
			return new NumericalSortPredicate(base.Column, false);
		}

		protected override SortPredicate CreateDescendingSortPredicate()
		{
			return new NumericalSortPredicate(base.Column, true);
		}

		private class NumericalSortPredicate : SortPredicate
		{
			public readonly bool Descending;

			public NumericalSortPredicate(StudyFilterColumn column, bool descending)
				: base(column)
			{
				this.Descending = descending;
			}

			public override int Compare(StudyItem x, StudyItem y)
			{
				if (this.Descending)
				{
					StudyItem z = y;
					y = x;
					x = z;
				}
				return ((INumericSortableColumn) base.Column).CompareNumerically(x, y);
			}
		}
	}

	[MenuAction("sortUp", "studyfilters-columnfilters/MenuSortAscendingSpatially", "SortAscending")]
	[IconSet("sortUp", IconScheme.Colour, "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png", "Icons.SortAscendingSmall.png")]
	[VisibleStateObserver("sortUp", "Visible", "VisibleChanged")]
	[MenuAction("sortDown", "studyfilters-columnfilters/MenuSortDescendingSpatially", "SortDescending")]
	[IconSet("sortDown", IconScheme.Colour, "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png", "Icons.SortDescendingSmall.png")]
	[VisibleStateObserver("sortDown", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class SpatialColumnSortTool : ColumnSortTool
	{
		protected override bool IsColumnSupported()
		{
			return base.Column is ISpatialSortableColumn;
		}

		protected override SortPredicate CreateAscendingSortPredicate()
		{
			return new SpatialSortPredicate(base.Column, false);
		}

		protected override SortPredicate CreateDescendingSortPredicate()
		{
			return new SpatialSortPredicate(base.Column, true);
		}

		private class SpatialSortPredicate : SortPredicate
		{
			public readonly bool Descending;

			public SpatialSortPredicate(StudyFilterColumn column, bool descending)
				: base(column)
			{
				this.Descending = descending;
			}

			public override int Compare(StudyItem x, StudyItem y)
			{
				if (this.Descending)
				{
					StudyItem z = y;
					y = x;
					x = z;
				}
				return ((ISpatialSortableColumn) base.Column).CompareSpatially(x, y);
			}
		}
	}
}