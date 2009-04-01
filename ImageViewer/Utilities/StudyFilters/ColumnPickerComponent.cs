using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class ColumnPickerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ColumnPickerComponentViewExtensionPoint))]
	public class ColumnPickerComponent : ApplicationComponent, IStudyFilterColumnCollectionCallbacks
	{
		private readonly StudyFilterColumnCollection _columns;

		public ColumnPickerComponent()
		{
			_columns = new StudyFilterColumnCollection(this);
		}

		public ColumnPickerComponent(IEnumerable<StudyFilterColumn> columns) : this()
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