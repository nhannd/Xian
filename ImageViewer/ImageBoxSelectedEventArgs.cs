#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.ImageBoxSelected"/> event.
	/// </summary>
	public class ImageBoxSelectedEventArgs : EventArgs
	{
		private IImageBox _selectedImageBox;

		internal ImageBoxSelectedEventArgs(
			IImageBox selectedImageBox)
		{
			Platform.CheckForNullReference(selectedImageBox, "selectedImageBox");
			_selectedImageBox = selectedImageBox;
		}

		/// <summary>
		/// Gets the selected <see cref="IImageBox"/>.
		/// </summary>
		public IImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
		}
	}
}
