using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DisplayedDumpChangedEventArgs : EventArgs
    {
        public DisplayedDumpChangedEventArgs(bool IsCurrentFirst, bool IsCurrentLast, bool IsCurrentTheOnly, bool HasCurrentBeenEditted)
        {
            _isCurrentFirst = IsCurrentFirst;
            _isCurrentLast = IsCurrentLast;
            _isCurrentTheOnly = IsCurrentTheOnly;
            _hasCurrentBeenEditted = HasCurrentBeenEditted;
        }

        public bool IsCurrentFirst
        {
            get { return _isCurrentFirst; }
        }

        public bool IsCurrentLast
        {
            get { return _isCurrentLast; }
        }

        public bool IsCurrentTheOnly
        {
            get { return _isCurrentTheOnly; }
        }

        public bool HasCurrentBeenEditted
        {
            get { return _hasCurrentBeenEditted; }
        }

        private bool _isCurrentFirst;
        private bool _isCurrentLast;
        private bool _isCurrentTheOnly;
        private bool _hasCurrentBeenEditted;

    }
}
