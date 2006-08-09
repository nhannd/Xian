using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.RebuildDatabase
{
    public class DatabaseRebuildCompletedEventArgs : EventArgs
    {
        public DatabaseRebuildCompletedEventArgs(bool rebuildWasAborted)
        {
            _rebuildWasAborted = rebuildWasAborted;
        }

        public bool RebulidWasAborted
        {
            get { return _rebuildWasAborted; }
        }

        private bool _rebuildWasAborted = false;
    }
}
