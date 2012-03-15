#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteTable : Table<OrderNoteDetail>
	{
		private readonly TableColumn<OrderNoteDetail, string> _linkColumn;

		public OrderNoteTable()
			: base(2)
		{
			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnAuthor, n => n.Author == null ? SR.LabelMe : StaffNameAndRoleFormat.Format(n.Author), 0.25f));
			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnPostTime, n => n.PostTime == null ? SR.LabelNew : Format.DateTime(n.PostTime), 0.25f));
			this.Columns.Add(_linkColumn = new TableColumn<OrderNoteDetail, string>(" ", n => SR.ColumnMore, 0.05f));
			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnComments, n => RemoveLineBreak(n.NoteBody), 0.5f, 1));
		}

		public Action<OrderNoteDetail> UpdateNoteClickLinkDelegate
		{
			get { return _linkColumn.ClickLinkDelegate; }
			set { _linkColumn.ClickLinkDelegate = value; }
		}

		private static string RemoveLineBreak(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			var newString = input.Replace("\r\n", " ");
			newString = newString.Replace("\r", " ");
			newString = newString.Replace("\n", " ");
			return newString;
		}
	}
}
