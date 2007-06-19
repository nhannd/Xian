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

        private event EventHandler _itemCheckAccepted;
        private event EventHandler _itemCheckRejected;

        public TechnologistDocumentationTable()
            : base(1)
        {
            this.Columns.Add(new TableColumn<TechnologistDocumentationTableItem, bool>("Selected",
                delegate(TechnologistDocumentationTableItem d) { return d.Selected; },
                delegate(TechnologistDocumentationTableItem d, bool value)
                   {
                       if (d.CanSelect)
                       {
                           d.Selected = value;
                           EventsHelper.Fire(_itemCheckAccepted, this, new ItemCheckedEventArgs(d));
                       }
                       else
                       {
                           EventsHelper.Fire(_itemCheckRejected, this, new ItemCheckedEventArgs(d));
                       }
                   },
               0.5f));

            this.Columns.Add(new TableColumn<TechnologistDocumentationTableItem, string>("Name",
                delegate(TechnologistDocumentationTableItem d) { return d.ProcedureStep.Name; },
                7.0f));

            this.Columns.Add(new TableColumn<TechnologistDocumentationTableItem, string>("Status",
                delegate(TechnologistDocumentationTableItem d) { return d.ProcedureStep.Status; },
                1.0f));

            this.BackgroundColorSelector = 
                delegate(object o) { return (((TechnologistDocumentationTableItem)o).CanSelect) ? "Empty" : "DimGray"; };
            this.OutlineColorSelector =
                delegate(object o) { return (((TechnologistDocumentationTableItem)o).CanSelect) ? "Empty" : "Red"; };
        }

        public event EventHandler ItemCheckAccepted
        {
            add { _itemCheckAccepted += value;  }
            remove { _itemCheckAccepted -= value; }
        }

        public event EventHandler ItemCheckRejected
        {
            add { _itemCheckRejected += value; }
            remove { _itemCheckRejected -= value; }
        }
    }
}