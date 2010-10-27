#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Wrapper class for items that are "checkable".
	/// </summary>
	/// <typeparam name="TItem">The type of the checkable item.</typeparam>
    public class Checkable<TItem>
    {
        private bool _isChecked;
        private TItem _item;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The checkable item.</param>
		/// <param name="isChecked">The initial check state of the item.</param>
        public Checkable(TItem item, bool isChecked)
        {
            _isChecked = isChecked;
            _item = item;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// The initial check state is false by default.
		/// </remarks>
		/// <param name="item">The checkable item.</param>
        public Checkable(TItem item)
            : this(item, false)
        {
        }

		/// <summary>
		/// Gets or sets the checkable item.
		/// </summary>
        public TItem Item
        {
            get { return _item; }
        }

		/// <summary>
		/// Gets or sets the check state of the item.
		/// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }
    }
}
