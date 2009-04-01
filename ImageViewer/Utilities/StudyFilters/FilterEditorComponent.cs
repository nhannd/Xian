using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class FilterEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(FilterEditorComponentViewExtensionPoint))]
	public class FilterEditorComponent : ApplicationComponent, IStudyFilterColumnCollectionCallbacks
	{
		private readonly StudyFilterColumnCollection _availableColumns;
		private readonly List<FilterNodeBase> _filters;

		public FilterEditorComponent()
		{
			_availableColumns = new StudyFilterColumnCollection(this);
			_filters = new List<FilterNodeBase>();
		}

		public FilterEditorComponent(IEnumerable<StudyFilterColumn> columns, IEnumerable<FilterNodeBase> filters)
			: this()
		{
			foreach (StudyFilterColumn column in columns)
			{
				_availableColumns.Add(column);
			}

			_filters.AddRange(filters);
		}

		public StudyFilterColumnCollection AvailableColumns
		{
			get { return _availableColumns; }
		}

		public IList<FilterNodeBase > Filters
		{
			get { return _filters; }
		}

		#region IStudyFilterColumnCollectionCallbacks Members

		void IStudyFilterColumnCollectionCallbacks.ColumnInserted(int index, StudyFilterColumn newColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnRemoved(int index, StudyFilterColumn oldColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnChanged(int index, StudyFilterColumn oldColumn, StudyFilterColumn newColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnsChanged(IEnumerable<StudyFilterColumn> newColumns) {}

		#endregion
	}
}