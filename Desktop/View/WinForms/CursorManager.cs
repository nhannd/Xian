#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public class CursorManager : IDisposable
    {
        private bool _disposed = false;
        private Control _owner;
        private Cursor _prevCursor;
        public CursorManager(Cursor cursor)
            : this(null, cursor)
        {
        }

        public CursorManager(Control owner, Cursor cursor)
        {
            _owner = owner;
            if (_owner != null)
            {
                _prevCursor = _owner.Cursor;
                _owner.Cursor = cursor;
            }
            else
            {
                _prevCursor = Cursor.Current;
                Cursor.Current = cursor;
            }
        }

        ~CursorManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_owner != null)
                    _owner.Cursor = _prevCursor;
                else
                    Cursor.Current = _prevCursor;

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
