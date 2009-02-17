#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class BiographyNoteTable : Table<PatientNoteDetail>
    {
        private static readonly int NumRows = 2;
        private static readonly int NoteCommentRow = 1;

        public BiographyNoteTable()
            : this(NumRows)
        {
        }

        private BiographyNoteTable(int cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<PatientNoteDetail, string>("Severity",
                delegate(PatientNoteDetail n) { return (n.Category == null ? "" : n.Category.Severity.Code); }, 0.05f));
            this.Columns.Add(new TableColumn<PatientNoteDetail, string>("Category",
                delegate(PatientNoteDetail n) { return (n.Category == null ? "" : n.Category.Name); }, 0.2f));
            this.Columns.Add(new TableColumn<PatientNoteDetail, string>("Description",
                delegate(PatientNoteDetail n) { return (n.Category == null ? "" : n.Category.Description); }, 0.4f));
            this.Columns.Add(new TableColumn<PatientNoteDetail, string>("Created By",
                delegate(PatientNoteDetail n) { return PersonNameFormat.Format(n.Author.Name); }, 0.2f));
            this.Columns.Add(new DateTableColumn<PatientNoteDetail>(SR.ColumnCreatedOn,
                delegate(PatientNoteDetail n) { return n.CreationTime; }, 0.1f));
			this.Columns.Add(new DateTableColumn<PatientNoteDetail>("Valid From",
				delegate(PatientNoteDetail n) { return n.ValidRangeFrom; }, 0.1f));
			this.Columns.Add(new DateTableColumn<PatientNoteDetail>("Valid Until",
				delegate(PatientNoteDetail n) { return n.ValidRangeUntil; }, 0.1f));
            this.Columns.Add(new TableColumn<PatientNoteDetail, string>("Comment",
                delegate(PatientNoteDetail n) { return (n.Comment != null && n.Comment.Length > 0 ? String.Format("Comment: {0}", n.Comment) : ""); }, 0.1f, NoteCommentRow));
        }
    }
}
