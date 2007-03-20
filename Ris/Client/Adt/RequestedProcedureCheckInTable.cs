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
    class RequestedProcedureCheckInTableEntry
    {
        private bool _checked = false;
        private CheckInTableItem _checkInTableItem;
        private event EventHandler _checkedChanged;

        public RequestedProcedureCheckInTableEntry(CheckInTableItem item)
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

    class RequestedProcedureCheckInTable : Table<RequestedProcedureCheckInTableEntry>
    {
        public RequestedProcedureCheckInTable()
        {
            this.Columns.Add(
               new TableColumn<RequestedProcedureCheckInTableEntry, bool>("Check-In",
                   delegate(RequestedProcedureCheckInTableEntry entry) { return entry.Checked; },
                   delegate(RequestedProcedureCheckInTableEntry entry, bool value) { entry.Checked = value; }, 0.30f));
            this.Columns.Add(
                new TableColumn<RequestedProcedureCheckInTableEntry, string>("Requested Procedures",
                    delegate(RequestedProcedureCheckInTableEntry entry) { return Format.Custom(entry.CheckInTableItem.RequestedProcedureName); }, 1.0f));
            this.Columns.Add(
                new TableColumn<RequestedProcedureCheckInTableEntry, string>("Scheduling Date",
                delegate(RequestedProcedureCheckInTableEntry entry) { return Format.Date(entry.CheckInTableItem.SchedulingDate); }, 0.5f));
            this.Columns.Add(
                new TableColumn<RequestedProcedureCheckInTableEntry, string>("Ordering Facility",
                delegate(RequestedProcedureCheckInTableEntry entry) { return Format.Custom(entry.CheckInTableItem.OrderingFacility); }, 0.5f));
        }
    }
}
