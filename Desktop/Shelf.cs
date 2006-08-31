using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract class that provides the base implementation of <see cref="IShelf"/>.
    /// </summary>
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

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
        protected virtual void Dispose(bool disposing)
        {
            // nothing to do
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
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(e);
            }
        }

        #endregion
    }
}
