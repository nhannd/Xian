#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public class FeedbackDetail
    {
        public FeedbackDetail(string category, string subject, string comments)
        {
            this.Category = category;
            this.Subject = subject;
            this.Comments = comments;
            this.CreatedOn = Platform.Time;
        }

        public string Category;
        public string Subject;
        public string Comments;
        public DateTime? CreatedOn;
    }

    public class BiographyFeedbackTable : DecoratedTable<FeedbackDetail>
    {
        private static readonly uint NumRows = 2;
        private static readonly uint NoteCommentRow = 1;

        public BiographyFeedbackTable()
            : this(NumRows)
        {
        }

        private BiographyFeedbackTable(uint cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<FeedbackDetail, string>("Category",
                delegate(FeedbackDetail f) { return (f.Category == null ? "" : f.Category); }, 0.2f));
            this.Columns.Add(new TableColumn<FeedbackDetail, string>("Subject",
                delegate(FeedbackDetail f) { return (f.Category == null ? "" : f.Subject); }, 0.4f));
            this.Columns.Add(new TableColumn<FeedbackDetail, string>(SR.ColumnCreatedOn,
                delegate(FeedbackDetail f) { return Format.Date(f.CreatedOn); }, 0.1f));
            this.Columns.Add(new DecoratedTableColumn<FeedbackDetail, string>("Comment",
                delegate(FeedbackDetail f) { return (f.Comments != null && f.Comments.Length > 0 ? String.Format("Comment: {0}", f.Comments) : ""); }, 0.1f, NoteCommentRow));
        }
    }
}
