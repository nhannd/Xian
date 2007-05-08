using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Common.Utilities;
using System.Drawing;

namespace ClearCanvas.Ris.Client
{
    public class NoteTable : Table<NoteDetail>
    {
        public NoteTable()
        {
            this.Columns.Add(new TableColumn<NoteDetail, string>("Severity",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Severity.Code); },
                0.1f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Category",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Name); },
                0.2f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Comment",
                delegate(NoteDetail n) { return n.Comment; },
                0.45f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Created",
                delegate(NoteDetail n) { return Format.Date(n.TimeStamp); },
                0.2f));
            //this.Columns.Add(new TableColumn<NoteDetail, string>("Description",
            //    delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Description); },
            //    1.0f));
        }
    }
}
