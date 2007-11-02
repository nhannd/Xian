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

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    class ModalityWorklistTable : Table<ModalityWorklistItem>
    {
        private static readonly uint NumRows = 2;
        private static readonly uint ProcedureDescriptionRow = 1;

        public ModalityWorklistTable()
            : this(NumRows)
        {
        }

        private ModalityWorklistTable(uint cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>(SR.ColumnMRN,
                delegate(ModalityWorklistItem item) { return MrnFormat.Format(item.Mrn); }, 
                0.5f));

            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>(SR.ColumnName,
                delegate(ModalityWorklistItem item) { return PersonNameFormat.Format(item.PersonNameDetail); },
                1.5f));

            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>(SR.ColumnAccessionNumber,
                delegate(ModalityWorklistItem item) { return item.AccessionNumber; },
                0.5f));

            TableColumn<ModalityWorklistItem, string> priorityColumn = new TableColumn<ModalityWorklistItem, string>(SR.ColumnPriority,
                delegate(ModalityWorklistItem item) { return item.Priority.Value; },
                0.5f);
            priorityColumn.Visible = false;
            this.Columns.Add(priorityColumn);

            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>("Procedure Description",
                delegate(ModalityWorklistItem item) 
                { 
                    return string.Format("{0} - {1}", item.RequestedProcedureTypeName, item.ModalityProcedureStepName); 
                },
                0.5f,
                ProcedureDescriptionRow));

            this.OutlineColorSelector = delegate(object o)
            {
                ModalityWorklistItem item = o as ModalityWorklistItem;
                if (item != null)
                {
                    switch (item.Priority.Code)
                    {
                        case "S":
                            return "Red";
                        case "A":
                            return "Yellow";
                        case "R":
                        default:
                            return "Empty";
                    }
                }
                else
                {
                    return "Empty";
                }
            };

            //this.BackgroundColorSelector = delegate(object o)
            //{
            //    ModalityWorklistItem item = o as ModalityWorklistItem;
            //    if (item != null)
            //    {
            //        switch (item.Priority.Code)
            //        {
            //            case "S":
            //                return "Red";
            //            case "A":
            //                return "Yellow";
            //            case "R":
            //            default:
            //                return "Empty";
            //        }
            //    }
            //    else
            //    {
            //        return "Empty";
            //    }
            //};
        }
    }
}
