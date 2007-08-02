using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class TechnologistDocumentationTable : DecoratedTable<TechnologistDocumentationTableItem>
    {

        public class ItemCheckedEventArgs : EventArgs
        {
            private readonly TechnologistDocumentationTableItem _item;

            public ItemCheckedEventArgs(TechnologistDocumentationTableItem _item)
            {
                this._item = _item;
            }

            public TechnologistDocumentationTableItem Item
            {
                get { return _item; }
            }
        }

        private event EventHandler _itemSelected;
        private event EventHandler _itemDeselected;
        private event EventHandler _itemSelectionRejected;

        public TechnologistDocumentationTable()
            : base(1)
        {
            this.Columns.Add(new TableColumn<TechnologistDocumentationTableItem, bool>(
                "Active?",
                delegate(TechnologistDocumentationTableItem d) { return d.Selected; },
                delegate(TechnologistDocumentationTableItem d, bool value)
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

            this.Columns.Add(new TableColumn<TechnologistDocumentationTableItem, string>(
                "Name",
                delegate(TechnologistDocumentationTableItem d) { return d.ProcedureStep.Name; },
                7.0f));

            TableColumn<TechnologistDocumentationTableItem, string> statusColumn = 
                new TableColumn<TechnologistDocumentationTableItem, string>(
                    "Status",
                    delegate(TechnologistDocumentationTableItem d) { return d.ProcedureStep.Status.Value; },
                    1.0f);
            this.Columns.Add(statusColumn);

            TableColumn<TechnologistDocumentationTableItem, DateTime> startTimeColumn = 
                new TableColumn<TechnologistDocumentationTableItem, DateTime>(
                    "Start Time",
                    delegate(TechnologistDocumentationTableItem d) 
                    {
                        if (d.ProcedureStep.PerformedProcedureStep == null) return DateTime.MinValue;
                        return d.ProcedureStep.PerformedProcedureStep.StartTime;  
                    },
                    1.0f);
            this.Columns.Add(startTimeColumn);

            TableColumn<TechnologistDocumentationTableItem, DateTime> endTimeColumn = 
                new TableColumn<TechnologistDocumentationTableItem, DateTime>(
                    "End Time",
                    delegate(TechnologistDocumentationTableItem d)
                    {
                        if (d.ProcedureStep.PerformedProcedureStep == null) return DateTime.MinValue;
                        return d.ProcedureStep.PerformedProcedureStep.EndTime ?? DateTime.MinValue;
                    },
                    1.0f);
            this.Columns.Add(endTimeColumn);

            this.Sort(new TableSortParams(endTimeColumn, true));

            this.BackgroundColorSelector = 
                delegate(object o) { return (((TechnologistDocumentationTableItem)o).CanSelect) ? "Empty" : "LightCyan"; };
            
            this.OutlineColorSelector =
                delegate(object o) { return (((TechnologistDocumentationTableItem)o).CanSelect) ? "Empty" : "Red"; };
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