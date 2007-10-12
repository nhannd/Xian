#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
                new TableColumn<CancelOrderTableEntry, string>(SR.ColumnRequestedProcedures,
                    delegate(CancelOrderTableEntry entry) { return entry.CancelOrderTableItem.RequestedProcedureNames; }, 1.5f));
            this.Columns.Add(
                new TableColumn<CancelOrderTableEntry, string>(SR.ColumnScheduledRequestDate,
                    delegate(CancelOrderTableEntry entry) { return Format.Date(entry.CancelOrderTableItem.SchedulingRequestDate); }, 0.75f));
            this.Columns.Add(
                new TableColumn<CancelOrderTableEntry, string>(SR.ColumnPriority,
                    delegate(CancelOrderTableEntry entry) { return entry.CancelOrderTableItem.Priority.Value; }, 0.3f));
        }
    }
}
