#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class ColumnPickerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ColumnPickerComponentViewExtensionPoint))]
	public class ColumnPickerComponent : ApplicationComponent
	{
		private readonly List<StudyFilterColumn.ColumnDefinition> _columns;

		public ColumnPickerComponent()
		{
			_columns = new List<StudyFilterColumn.ColumnDefinition>();
		}

		public ColumnPickerComponent(IEnumerable<StudyFilterColumn> columns) : this()
		{
			foreach (StudyFilterColumn column in columns)
			{
				_columns.Add(StudyFilterColumn.GetColumnDefinition(column.Key));
			}
		}

		public IList<StudyFilterColumn.ColumnDefinition> Columns
		{
			get { return _columns; }
		}
	}
}