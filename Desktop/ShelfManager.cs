using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class ShelfManager : IDisposable
    {
        private ShelfCollection _shelves;
        private IDesktopWindow _desktopWindow;

        /// <summary>
        /// Constructor
        /// </summary>
        internal ShelfManager(IDesktopWindow desktopWindow)
	    {
            _shelves = new ShelfCollection(this);
            _desktopWindow = desktopWindow;
	    }

        ~ShelfManager()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            foreach (IShelf shelf in _shelves)
            {
                shelf.Dispose();
            }
            _shelves.Clear();
        }

        public ShelfCollection Shelves
        {
            get { return _shelves; }
        }

        internal void ShelfAdded(IShelf shelf)
        {
            shelf.Initialize(_desktopWindow);
        }

        internal void ShelfRemoved(IShelf shelf)
        {
            // dispose of the shelf
            shelf.Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
