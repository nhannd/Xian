using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public abstract class Shelf : IShelf
    {
        private IDesktopWindow _desktopWindow;
        private EventHandler _titleChanged;
        private string _title;
        private ShelfDisplayHint _displayHint;

        public Shelf(string title, ShelfDisplayHint displayHint)
        {
            _title = title;
            _displayHint = displayHint;
        }

        ~Shelf()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #region IShelf Members

        public virtual void Initialize(IDesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }

        public IDesktopWindow DesktopWindow
        {
            get { return _desktopWindow; }
        }

        public event EventHandler TitleChanged
        {
            add { _titleChanged += value; }
            remove { _titleChanged -= value; }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    EventsHelper.Fire(_titleChanged, this, new EventArgs());
                }
            }
        }

        public ShelfDisplayHint DisplayHint
        {
            get { return _displayHint; }
            protected set { _displayHint = value; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
