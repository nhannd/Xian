using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class Checkable<TItem>
    {
        private bool _isChecked;
        private TItem _item;

        public Checkable(TItem item, bool isChecked)
        {
            _isChecked = isChecked;
            _item = item;
        }

        public Checkable(TItem item)
            : this(item, false)
        {
        }

        public TItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }
    }
}
