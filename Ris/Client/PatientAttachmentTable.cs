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

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class PatientAttachmentTable : Table<PatientAttachmentSummary>
    {
        public PatientAttachmentTable()
        {
            this.Columns.Add(new DateTableColumn<PatientAttachmentSummary>(SR.ColumnDate,
                delegate(PatientAttachmentSummary summary) { return summary.Document.CreationTime; }, 0.2f));
            this.Columns.Add(new TableColumn<PatientAttachmentSummary, string>(SR.ColumnCategory,
                delegate(PatientAttachmentSummary summary) { return summary.Category.Value; }, 0.2f));
            this.Columns.Add(new TableColumn<PatientAttachmentSummary, string>(SR.ColumnAttachedBy,
                delegate(PatientAttachmentSummary summary) { return summary.AttachedBy == null ? "me" : PersonNameFormat.Format(summary.AttachedBy.Name); }, 0.2f));
            this.Columns.Add(new TableColumn<PatientAttachmentSummary, string>(SR.ColumnAttachmentType,
                delegate(PatientAttachmentSummary summary) { return summary.Document.DocumentTypeName; }, 0.2f));
            
            // Sort the table by descending date initially
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<PatientAttachmentSummary> column)
                { return column.Name.Equals(SR.ColumnDate); });

            this.Sort(new TableSortParams(this.Columns[sortColumnIndex], false));
        }
    }
}
