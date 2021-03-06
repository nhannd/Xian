#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop {
	/// <summary>
	/// Represents the method that handles an event involving a single <see cref="IGalleryItem"/>.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="GalleryItemEventArgs"/> that contains the event data.</param>
	public delegate void GalleryItemEventHandler(object sender, GalleryItemEventArgs e);

	/// <summary>
	/// Provides data for an event involving a single <see cref="IGalleryItem"/>.
	/// </summary>
	public class GalleryItemEventArgs : EventArgs {
		private readonly IGalleryItem _item;
		
		/// <summary>
		/// Constructs a new <see cref="GalleryItemEventArgs"/>.
		/// </summary>
		/// <param name="item">The <see cref="IGalleryItem"/>.</param>
		public GalleryItemEventArgs(IGalleryItem item)
		{
			_item = item;
		}

		/// <summary>
		/// Gets the <see cref="IGalleryItem"/>.
		/// </summary>
		public IGalleryItem Item
		{
			get { return _item; }
		}
	}
}
