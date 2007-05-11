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
