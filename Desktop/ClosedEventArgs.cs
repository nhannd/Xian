using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    public class ClosedEventArgs : EventArgs
    {
        private CloseReason _reason;

        public ClosedEventArgs(CloseReason reason)
        {
            _reason = reason;
        }

        public CloseReason Reason
        {
            get { return _reason; }
        }
    }

    public class ClosedItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private CloseReason _reason;

        public ClosedItemEventArgs(TItem item, CloseReason reason)
            :base(item)
        {
            _reason = reason;
        }

        public CloseReason Reason
        {
            get { return _reason; }
        }
    }
}
