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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class XTechnologistDocumentationTable : DecoratedTable<XTechnologistDocumentationTableItem>
    {

        public class ItemCheckedEventArgs : EventArgs
        {
            private readonly XTechnologistDocumentationTableItem _item;

            public ItemCheckedEventArgs(XTechnologistDocumentationTableItem _item)
            {
                this._item = _item;
            }

            public XTechnologistDocumentationTableItem Item
            {
                get { return _item; }
            }
        }

        private event EventHandler _itemSelected;
        private event EventHandler _itemDeselected;
        private event EventHandler _itemSelectionRejected;

        public XTechnologistDocumentationTable()
            : base(1)
        {
            this.Columns.Add(new TableColumn<XTechnologistDocumentationTableItem, bool>(
                "Active?",
                delegate(XTechnologistDocumentationTableItem d) { return d.Selected; },
                delegate(XTechnologistDocumentationTableItem d, bool value)
                   {
                       if (d.CanSelect)
                       {
                           d.Selected = value;
                           if(value)
                               EventsHelper.Fire(_itemSelected, this, new ItemCheckedEventArgs(d));
                           else 
                               EventsHelper.Fire(_itemDeselected, this, new ItemCheckedEventArgs(d));
                       }
                       else
                       {
                           EventsHelper.Fire(_itemSelectionRejected, this, new ItemCheckedEventArgs(d));
                       }
                   },
               0.5f));

            this.Columns.Add(new TableColumn<XTechnologistDocumentationTableItem, string>(
                "Name",
                delegate(XTechnologistDocumentationTableItem d) { return d.ProcedureStep.Name; },
                7.0f));

            TableColumn<XTechnologistDocumentationTableItem, string> statusColumn = 
                new TableColumn<XTechnologistDocumentationTableItem, string>(
                    "Status",
                    delegate(XTechnologistDocumentationTableItem d) { return d.ProcedureStep.Status.Value; },
                    1.0f);
            this.Columns.Add(statusColumn);

            TableColumn<XTechnologistDocumentationTableItem, DateTime> startTimeColumn = 
                new TableColumn<XTechnologistDocumentationTableItem, DateTime>(
                    "Start Time",
                    delegate(XTechnologistDocumentationTableItem d) 
                    {
                        if (d.ProcedureStep.PerformedProcedureStep == null) return DateTime.MinValue;
                        return d.ProcedureStep.PerformedProcedureStep.StartTime;  
                    },
                    1.0f);
            this.Columns.Add(startTimeColumn);

            TableColumn<XTechnologistDocumentationTableItem, DateTime> endTimeColumn = 
                new TableColumn<XTechnologistDocumentationTableItem, DateTime>(
                    "End Time",
                    delegate(XTechnologistDocumentationTableItem d)
                    {
                        if (d.ProcedureStep.PerformedProcedureStep == null) return DateTime.MinValue;
                        return d.ProcedureStep.PerformedProcedureStep.EndTime ?? DateTime.MinValue;
                    },
                    1.0f);
            this.Columns.Add(endTimeColumn);

            this.Sort(new TableSortParams(endTimeColumn, true));

            this.BackgroundColorSelector = 
                delegate(object o) { return (((XTechnologistDocumentationTableItem)o).CanSelect) ? "Empty" : "LightCyan"; };
            
            this.OutlineColorSelector =
                delegate(object o) { return (((XTechnologistDocumentationTableItem)o).CanSelect) ? "Empty" : "Red"; };
        }

        public event EventHandler ItemSelected
        {
            add { _itemSelected += value;  }
            remove { _itemSelected -= value; }
        }

        public event EventHandler ItemDeselected
        {
            add { _itemDeselected += value; }
            remove { _itemDeselected -= value; }
        }

        public event EventHandler ItemSelectionRejected
        {
            add { _itemSelectionRejected += value; }
            remove { _itemSelectionRejected -= value; }
        }
    }
}