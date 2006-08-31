using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Event args used for "closing" events, where the request may need to be cancelled
    /// </summary>
    public class ClosingEventArgs : EventArgs
    {
        private bool _cancel;

        /// <summary>
        /// Flag that controls whether the close request should be cancelled
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }
    }
}
