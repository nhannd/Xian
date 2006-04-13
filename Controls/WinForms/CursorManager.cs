namespace ClearCanvas.Controls.WinForms
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

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
