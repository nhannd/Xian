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