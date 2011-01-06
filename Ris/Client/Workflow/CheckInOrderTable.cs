#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    class CheckInOrderTableEntry
    {
        private bool _checked = true;
        private readonly ProcedureSummary _procedure;
        private event EventHandler _checkedChanged;

        public CheckInOrderTableEntry(ProcedureSummary item)
        {
            _procedure = item;
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

        public ProcedureSummary Procedure
        {
            get { return _procedure; }
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
               new TableColumn<CheckInOrderTableEntry, bool>(SR.ColumnCheckIn,
                   delegate(CheckInOrderTableEntry entry) { return entry.Checked; },
                   delegate(CheckInOrderTableEntry entry, bool value) { entry.Checked = value; }, 0.30f));
            this.Columns.Add(
                new TableColumn<CheckInOrderTableEntry, string>(SR.ColumnProcedures,
                    delegate(CheckInOrderTableEntry entry) { return ProcedureFormat.Format(entry.Procedure); }, 1.0f));
            this.Columns.Add(
                new DateTimeTableColumn<CheckInOrderTableEntry>(SR.ColumnTime,
                delegate(CheckInOrderTableEntry entry) { return entry.Procedure.ScheduledStartTime; }, 0.5f));
            this.Columns.Add(
                new TableColumn<CheckInOrderTableEntry, string>(SR.ColumnPerformingFacility,
                delegate(CheckInOrderTableEntry entry) { return entry.Procedure.PerformingFacility.Name; }, 0.5f));
        }
    }
}
