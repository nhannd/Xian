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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Admin
{
    public class HL7QueueItemSummaryTable : Table<HL7QueueItemSummary>
    {
        public HL7QueueItemSummaryTable()
        {
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnDirection,
                    delegate(HL7QueueItemSummary item) { return item.Direction; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnPeer,
                    delegate(HL7QueueItemSummary item) { return item.Peer; }, 0.90f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnType,
                    delegate(HL7QueueItemSummary item) { return item.MessageType; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnStatus,
                    delegate(HL7QueueItemSummary item) { return item.StatusCode; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnStatusDescription,
                    delegate(HL7QueueItemSummary item) { return item.StatusDescription; }, 3.0f));
            this.Columns.Add(
				new DateTableColumn<HL7QueueItemSummary>(SR.ColumnCreatedOn,
                    delegate(HL7QueueItemSummary item) { return item.CreationTime; }, 1.5f));
            this.Columns.Add(
                new DateTimeTableColumn<HL7QueueItemSummary>(SR.ColumnUpdatedOn,
                    delegate(HL7QueueItemSummary item) { return item.UpdateTime; }, 1.5f));
        }
    }
}
