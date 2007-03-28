using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class NoteTable : Table<NoteDetail>
    {
        public NoteTable()
        {
            this.Columns.Add(new TableColumn<NoteDetail, string>("Text",
                delegate(NoteDetail n) { return n.Text; },
                1.0f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Date",
                delegate(NoteDetail n) { return Format.DateTime(n.TimeStamp); },
                1.0f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Severity",
                delegate(NoteDetail n) { return n.Severity; },
                1.0f));
            //this.Columsn.Add(new TableColumn<Staff, string>("Author",
            //    delegate(Note n) { n.StaffAuthor.ToString() },
            //    1.0f));
        }
    }
}
