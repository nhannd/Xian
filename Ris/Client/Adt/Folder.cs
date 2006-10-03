using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    public class Folder : IFolder
    {
        private string _name;
        private event EventHandler _itemsChanged;

        public Folder(string name)
        {
            _name = name;
        }

        protected void NotifyItemsChanged()
        {
            EventsHelper.Fire(_itemsChanged, this, EventArgs.Empty);
        }

        #region IFolder Members

        public string DisplayName
        {
            get { return _name; }
        }

        public virtual ITableData Items
        {
            get { return null; }
        }

        public event EventHandler ItemsChanged
        {
            add { _itemsChanged += value; }
            remove { _itemsChanged -= value; }
        }

        #endregion
    }
}
