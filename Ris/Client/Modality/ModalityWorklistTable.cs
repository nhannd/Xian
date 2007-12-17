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
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Modality
{
    public class ModalityWorklistTable : Table<WorklistQueryResult>
    {
        public ModalityWorklistTable(OrderPriorityEnumTable orderPriorities)
        {
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("MRN",
                delegate(WorklistQueryResult item) { return Format.Custom(item.Mrn); }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Name",
                delegate(WorklistQueryResult item) { return Format.Custom(item.PatientName); }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Accession #",
                delegate(WorklistQueryResult item) { return item.AccessionNumber; }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Procedure Step",
                delegate(WorklistQueryResult item) { return item.ModalityProcedureStepName; }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Modality",
                delegate(WorklistQueryResult item) { return item.ModalityName; }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Priority",
                delegate(WorklistQueryResult item) { return orderPriorities[item.Priority].Value; }));
        }
   }
}
