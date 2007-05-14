using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    class CancelOrderTableEntry
    {
        private bool _checked = false;
        private CancelOrderTableItem _cancelOrderTableItem;
        private event EventHandler _checkedChanged;

        public CancelOrderTableEntry(CancelOrderTableItem item)
        {
            _cancelOrderTableItem = item;
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    EventsHelper.Fire(_checkedChanged, this, EventArgs.Empty);
                }
            }
        }

        public CancelOrderTableItem CancelOrderTableItem
        {
            get { return _cancelOrderTableItem; }
        }

        public event EventHandler CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }
    }

    class CancelOrderTable : Table<CancelOrderTableEntry>
    {
        public CancelOrderTable()
        {
            this.Columns.Add(
               new TableColumn<CancelOrderTableEntry, bool>(SR.ColumnCancel,
                   delegate(CancelOrderTableEntry entry) { return entry.Checked; },
                   delegate(CancelOrderTableEntry entry, bool value) { entry.Checked = value; }, 0.3f));
            this.Columns.Add(
                new TableColumn<CancelOrderTableEntry, string>(SR.ColumnAccessionNumber,
                    delegate(CancelOrderTableEntry entry) { return entry.CancelOrderTableItem.AccessionNumber; }, 0.75f));
            this.Columns.Add(
                new TableColumn<CancelOrderTableEntry, string>(SR.ColumnDiagnosticService,
                    delegate(CancelOrderTableEntry entry) { return entry.CancelOrderTableItem.DiagnosticServiceName; }, 1.5f));
            this.Columns.Add(
                new TableColumn<CancelOrderTableEntry, string>(SR.ColumnScheduledRequestDate,
                    delegate(CancelOrderTableEntry entry) { return Format.Date(entry.CancelOrderTableItem.SchedulingRequestDate); }, 0.75f));
            this.Columns.Add(
                new TableColumn<CancelOrderTableEntry, string>(SR.ColumnPriority,
                    delegate(CancelOrderTableEntry entry) { return entry.CancelOrderTableItem.Priority.Value; }, 0.3f));
        }
    }
}
