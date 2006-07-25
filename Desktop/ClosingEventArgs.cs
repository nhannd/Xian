using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class ClosingEventArgs : EventArgs
    {
        private bool _cancel;

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }
    }
}
