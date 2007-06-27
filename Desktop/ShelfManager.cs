using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Manages a collection of <see cref="IShelf"/> objects.
    /// </summary>
    public class ShelfManager : IDisposable
    {
        private ShelfCollection _shelves;
        private IDesktopWindow _desktopWindow;
		private event EventHandler<ItemEventArgs<IShelf>> _shelfActivated;
		
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
            if (disposing && _shelves != null)
            {
                _shelves.Clear();
                _shelves = null;
            }
        }

		public event EventHandler<ItemEventArgs<IShelf>> ShelfActivated
		{
			add { _shelfActivated += value; }
			remove { _shelfActivated -= value; }
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

		internal void ActivateShelf(IShelf shelf)
		{
			EventsHelper.Fire(_shelfActivated, this, new ItemEventArgs<IShelf>(shelf));
		}
	}
}
