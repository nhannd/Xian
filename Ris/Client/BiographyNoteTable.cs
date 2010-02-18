#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class BiographyNoteTable : Table<PatientNoteDetail>
	{
		private const int NumRows = 2;
		private const int NoteCommentRow = 1;

		public BiographyNoteComponent _component;

		public BiographyNoteTable(BiographyNoteComponent component)
			: this(component, NumRows)
		{
		}

		private BiographyNoteTable(BiographyNoteComponent component, int cellRowCount)
			: base(cellRowCount)
		{
			_component = component;

			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnSeverity,
				n => (n.Category == null ? "" : n.Category.Severity.Value), 0.06f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnCategory,
				n => (n.Category == null ? "" : n.Category.Name), 0.2f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnDescription,
				n => (n.Category == null ? "" : n.Category.Description), 0.4f));
			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnAuthor,
				n => PersonNameFormat.Format(n.Author.Name), 0.2f));

			ITableColumn _createdOnColumn;
			this.Columns.Add(_createdOnColumn = new DateTimeTableColumn<PatientNoteDetail>(SR.ColumnCreatedOn,
				n => n.CreationTime, 0.1f));
			this.Columns.Add(new DateTableColumn<PatientNoteDetail>(SR.ColumnValidFrom,
				n => n.ValidRangeFrom, 0.1f));
			this.Columns.Add(new DateTableColumn<PatientNoteDetail>(SR.ColumnValidUntil,
				n => n.ValidRangeUntil, 0.1f));

			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(" ",
				n => SR.ColumnMore, 0.05f)
					{
						ClickLinkDelegate = component.ShowNoteDetail
					});

			this.Columns.Add(new TableColumn<PatientNoteDetail, string>(SR.ColumnComments,
				n => (string.IsNullOrEmpty(n.Comment) ? "" : String.Format("Comment: {0}", RemoveLineBreak(n.Comment))), 0.1f, NoteCommentRow));

			// there aren't any items to sort right now, but calling this sets the default sort parameters to "Created" column desc
			this.Sort(new TableSortParams(_createdOnColumn, false));
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
