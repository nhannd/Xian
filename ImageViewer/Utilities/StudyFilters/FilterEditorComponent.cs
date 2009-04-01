using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class FilterEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(FilterEditorComponentViewExtensionPoint))]
	public class FilterEditorComponent : ApplicationComponent, IStudyFilterColumnCollectionCallbacks
	{
		private readonly StudyFilterColumnCollection _columns;

		public FilterEditorComponent()
		{
			_columns = new StudyFilterColumnCollection(this);
		}

		public FilterEditorComponent(IEnumerable<StudyFilterColumn> columns)
			: this()
		{
			foreach (StudyFilterColumn column in columns)
			{
				_columns.Add(column);
			}
		}

		public StudyFilterColumnCollection Columns
		{
			get { return _columns; }
		}

		#region IStudyFilterColumnCollectionCallbacks Members

		void IStudyFilterColumnCollectionCallbacks.ColumnInserted(int index, StudyFilterColumn newColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnRemoved(int index, StudyFilterColumn oldColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnChanged(int index, StudyFilterColumn oldColumn, StudyFilterColumn newColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnsChanged(IEnumerable<StudyFilterColumn> newColumns) {}

		#endregion
	}
}