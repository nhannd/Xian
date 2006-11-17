using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DisplayedDumpChangedEventArgs : EventArgs
    {
        public DisplayedDumpChangedEventArgs(bool IsCurrentFirst, bool IsCurrentLast, bool IsCurrentTheOnly)
        {
            _isCurrentFirst = IsCurrentFirst;
            _isCurrentLast = IsCurrentLast;
            _isCurrentTheOnly = IsCurrentTheOnly;
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

        private bool _isCurrentFirst;
        private bool _isCurrentLast;
        private bool _isCurrentTheOnly;

    }
}
