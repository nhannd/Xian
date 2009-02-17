#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
