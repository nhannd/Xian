using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client
{
    public class NoteTable : Table<NoteDetail>
    {
        public NoteTable()
        {
            this.Columns.Add(new TableColumn<NoteDetail, string>("Category",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Name); },
                1.0f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Description",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Description); },
                1.0f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Severity",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Severity.Value); },
                1.0f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Comment",
                delegate(NoteDetail n) { return n.Comment; },
                1.0f));
        }
    }
}
