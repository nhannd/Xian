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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public enum DocumentCategory
    {
        LabResult,
        ScreeningForm,
        OutsideImagingReport,
        SurgicalReport,
        PatientConsentForm
    }
    
    public class DocumentSummary
    {
        public DocumentSummary()
        {
        }

        public DocumentSummary(string category, DateTime documentDate, string institution, string reason, int pages,
            string createdStaff, DateTime createdTime)
        {
            this.Category = category;
            this.DocumentDate = documentDate;
            this.Institution = institution;
            this.Reason = reason;
            this.Pages = pages;
            this.CreatedStaff = createdStaff;
            this.CreatedTime = createdTime;
        }

        public string Category;
        public DateTime? DocumentDate;
        public string Institution;
        public string Reason;
        public int Pages;

        public string CreatedStaff;
        public DateTime CreatedTime;
    }

    public class BiographyDocumentTable : Table<DocumentSummary>
    {
        public BiographyDocumentTable()
        {
            this.Columns.Add(new TableColumn<DocumentSummary, string>("Date",
                delegate(DocumentSummary summary) { return Format.Date(summary.DocumentDate); }, 0.2f));
            this.Columns.Add(new TableColumn<DocumentSummary, string>("Category",
                delegate(DocumentSummary summary) { return summary.Category; }, 0.2f));
            this.Columns.Add(new TableColumn<DocumentSummary, string>("Institution",
                delegate(DocumentSummary summary) { return summary.Institution; }, 0.2f));
            this.Columns.Add(new TableColumn<DocumentSummary, string>("Reason",
                delegate(DocumentSummary summary) { return summary.Reason; }, 0.4f));
            this.Columns.Add(new TableColumn<DocumentSummary, int>("Pages",
                delegate(DocumentSummary summary) { return summary.Pages; }, 0.05f));
        }
    }
}
