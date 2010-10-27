#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Client
{
	public class CannedTextTable : Table<CannedTextSummary>
	{
		public CannedTextTable()
		{
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnName, c => c.Name, 1.0f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnCategory, c => c.Category, 1.0f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnText, c => FormatCannedTextSnippet(c.TextSnippet), 3.0f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnCannedTextOwner,
				item => item.IsPersonal ? SR.ColumnPersonal : item.StaffGroup.Name, 1.0f));

			// Apply sort from settings
			var sortColumnIndex = this.Columns.FindIndex(column => column.Name.Equals(CannedTextSettings.Default.SummarySortColumnName));
			this.Sort(new TableSortParams(this.Columns[sortColumnIndex], CannedTextSettings.Default.SummarySortAscending));

			this.Sorted += OnCannedTextTableSorted;
		}

		private void OnCannedTextTableSorted(object sender, System.EventArgs e)
		{
			if (this.SortParams == null)
				return;

			// Save last sort
			CannedTextSettings.Default.SummarySortColumnName = this.SortParams.Column.Name;
			CannedTextSettings.Default.SummarySortAscending = this.SortParams.Ascending;
			CannedTextSettings.Default.Save();
		}

		private static string FormatCannedTextSnippet(string text)
		{
			return text.Length < CannedTextSummary.MaxTextLength ? text : string.Concat(text, "...");
		}
	}
}
