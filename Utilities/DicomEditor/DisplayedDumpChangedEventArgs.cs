#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DisplayedDumpChangedEventArgs : EventArgs
    {
        public DisplayedDumpChangedEventArgs(bool IsCurrentFirst, bool IsCurrentLast, bool IsCurrentTheOnly, bool hasCurrentBeenEdited)
        {
            _isCurrentFirst = IsCurrentFirst;
            _isCurrentLast = IsCurrentLast;
            _isCurrentTheOnly = IsCurrentTheOnly;
            _hasCurrentBeenEdited = hasCurrentBeenEdited;
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

        public bool HasCurrentBeenEdited
        {
            get { return _hasCurrentBeenEdited; }
        }

        private bool _isCurrentFirst;
        private bool _isCurrentLast;
        private bool _isCurrentTheOnly;
        private bool _hasCurrentBeenEdited;

    }
}
