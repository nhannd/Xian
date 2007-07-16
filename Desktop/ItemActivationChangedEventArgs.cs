using System;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides information about a change in workspace activation.
    /// </summary>
	public class ItemActivationChangedEventArgs<T> : EventArgs
	{
        private T _deactivatedItem;
        private T _activatedItem;

        public ItemActivationChangedEventArgs(T activatedItem, T deactivatedItem)
		{
            _activatedItem = activatedItem;
            _deactivatedItem = deactivatedItem;
        }

        /// <summary>
        /// Gets the item that was activated
        /// </summary>
        public T ActivatedItem
        {
            get { return _activatedItem; }
        }

        /// <summary>
        /// Gets the item that was deactivated
        /// </summary>
        public T DeactivatedItem
        {
            get { return _deactivatedItem; }
        }
	}
}
