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
	public class ImageBoxDrawingEventArgs : EventArgs
	{
		private IImageBox _imageBox;

		internal ImageBoxDrawingEventArgs(IImageBox imageBox)
		{
			Platform.CheckForNullReference(imageBox, "imageBox");
			_imageBox = imageBox;
		}

		/// <summary>
		/// Gets the selected <see cref="IImageBox"/>.
		/// </summary>
		public IImageBox ImageBox
		{
			get { return _imageBox; }
		}
	}
}