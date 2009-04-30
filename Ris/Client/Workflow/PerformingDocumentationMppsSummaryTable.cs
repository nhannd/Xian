#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class PerformingDocumentationMppsSummaryTable : Table<ModalityPerformedProcedureStepDetail>
    {
        public PerformingDocumentationMppsSummaryTable()
        {
            this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepDetail, string>(
                                 SR.ColumnName,
                                 delegate(ModalityPerformedProcedureStepDetail mpps) { return mpps.Description; },
                                 5.0f));

            this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepDetail, string>(
                                 SR.ColumnState,
                                 delegate(ModalityPerformedProcedureStepDetail mpps) { return FormatStatus(mpps); },
                                 1.2f));

			DateTimeTableColumn<ModalityPerformedProcedureStepDetail> sortColumn = 
                new DateTimeTableColumn<ModalityPerformedProcedureStepDetail>(
                                 SR.ColumnStartTime,
                                 delegate(ModalityPerformedProcedureStepDetail mpps) { return mpps.StartTime; },
                                 1.5f);

			this.Columns.Add(sortColumn);
			this.Sort(new TableSortParams(sortColumn, true));

            DateTimeTableColumn<ModalityPerformedProcedureStepDetail> endTimeColumn =
				new DateTimeTableColumn<ModalityPerformedProcedureStepDetail>(
                                 SR.ColumnEndTime,
                                 delegate(ModalityPerformedProcedureStepDetail mpps) { return mpps.EndTime; },
                                 1.5f);

			this.Columns.Add(endTimeColumn);
        }

        private static string FormatStatus(ModalityPerformedProcedureStepDetail mpps)
        {
            if (mpps.State.Code == "CM")
                return "Performed";
            else
                return mpps.State.Value;
        }
    }
}