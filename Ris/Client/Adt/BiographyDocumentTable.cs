using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
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
