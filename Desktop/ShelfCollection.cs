using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class ShelfCollection : ObservableList<IShelf, ShelfEventArgs>
    {
        private ShelfManager _owner;

        internal ShelfCollection(ShelfManager owner)
        {
            _owner = owner;
        }

        protected override void OnItemAdded(ShelfEventArgs e)
        {
            _owner.ShelfAdded(e.Item);
            base.OnItemAdded(e);
        }

        protected override void OnItemRemoved(ShelfEventArgs e)
        {
            _owner.ShelfRemoved(e.Item);
            base.OnItemRemoved(e);
        }
    }
}
