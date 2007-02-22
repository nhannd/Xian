using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    class RequestedProcedureCheckInTableEntry
    {
        private bool _checked = false;
        private RequestedProcedure _requestedProcedure;
        private event EventHandler _checkedChanged;

        public RequestedProcedureCheckInTableEntry(RequestedProcedure rp)
        {
            _requestedProcedure = rp;
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

        public RequestedProcedure RequestedProcedure
        {
            get { return _requestedProcedure; }
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
                   delegate(RequestedProcedureCheckInTableEntry item) { return item.Checked; },
                   delegate(RequestedProcedureCheckInTableEntry item, bool value) { item.Checked = value; }, 0.35f));
            this.Columns.Add(
                new TableColumn<RequestedProcedureCheckInTableEntry, string>("Requested Procedures",
                    delegate(RequestedProcedureCheckInTableEntry item) { return item.RequestedProcedure.Type.Name; }, 1.0f));
            this.Columns.Add(
                new TableColumn<RequestedProcedureCheckInTableEntry, DateTime>("Scheduling Time",
                delegate(RequestedProcedureCheckInTableEntry item) { return item.RequestedProcedure.Order.SchedulingRequestDateTime; }, 0.7f));
        }
    }
}
