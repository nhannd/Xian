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
    class CheckInOrderTableEntry
    {
        private bool _checked = false;
        private CheckInTableItem _checkInTableItem;
        private event EventHandler _checkedChanged;

        public CheckInOrderTableEntry(CheckInTableItem item)
        {
            _checkInTableItem = item;
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

        public CheckInTableItem CheckInTableItem
        {
            get { return _checkInTableItem; }
        }

        public event EventHandler CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }
    }

    class CheckInOrderTable : Table<CheckInOrderTableEntry>
    {
        public CheckInOrderTable()
        {
            this.Columns.Add(
               new TableColumn<CheckInOrderTableEntry, bool>("Check-In",
                   delegate(CheckInOrderTableEntry entry) { return entry.Checked; },
                   delegate(CheckInOrderTableEntry entry, bool value) { entry.Checked = value; }, 0.30f));
            this.Columns.Add(
                new TableColumn<CheckInOrderTableEntry, string>("Requested Procedures",
                    delegate(CheckInOrderTableEntry entry) { return entry.CheckInTableItem.RequestedProcedureNames; }, 1.0f));
            this.Columns.Add(
                new TableColumn<CheckInOrderTableEntry, string>("Scheduling Date",
                delegate(CheckInOrderTableEntry entry) { return Format.Date(entry.CheckInTableItem.SchedulingDate); }, 0.5f));
            this.Columns.Add(
                new TableColumn<CheckInOrderTableEntry, string>("Ordering Facility",
                delegate(CheckInOrderTableEntry entry) { return entry.CheckInTableItem.OrderingFacility; }, 0.5f));
        }
    }
}
