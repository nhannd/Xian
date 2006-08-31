using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Manages a collection of <see cref="IShelf"/> objects.
    /// </summary>
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

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IShelf shelf in _shelves)
                {
                    // important that we don't throw any exceptions from this method
                    try
                    {
                        shelf.Dispose();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(e);
                    }
                }
                _shelves.Clear();
            }
        }

        /// <summary>
        /// The collection of <see cref="IShelf"/> objects that are currently being managed
        /// </summary>
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
