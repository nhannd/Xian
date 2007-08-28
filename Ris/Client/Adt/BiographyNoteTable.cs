using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public class BiographyNoteTable : DecoratedTable<NoteDetail>
    {
        private static readonly uint NumRows = 2;
        private static readonly uint NoteCommentRow = 1;

        public BiographyNoteTable()
            : this(NumRows)
        {
        }

        private BiographyNoteTable(uint cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<NoteDetail, string>("Severity",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Severity.Code); }, 0.05f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Category",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Name); }, 0.2f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Description",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Description); }, 0.4f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Created By",
                delegate(NoteDetail n) { return String.Format("{0}, {1}", n.CreatedBy.Name.FamilyName, n.CreatedBy.Name.GivenName); }, 0.2f));
            this.Columns.Add(new TableColumn<NoteDetail, string>(SR.ColumnCreatedOn,
                delegate(NoteDetail n) { return Format.Date(n.TimeStamp); }, 0.1f));
            this.Columns.Add(new DecoratedTableColumn<NoteDetail, string>("Comment",
                delegate(NoteDetail n) { return (n.Comment != null && n.Comment.Length > 0 ? String.Format("Comment: {0}", n.Comment) : ""); }, 0.1f, NoteCommentRow));
        }
    }
}
