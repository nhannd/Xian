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
	/// Provides data for the <see cref="EventBroker.ImageDrawing"/> event.
	/// </summary>
	public class ImageDrawingEventArgs : EventArgs
	{
		private IPresentationImage _presentationImage;

		internal ImageDrawingEventArgs(IPresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");

			_presentationImage = presentationImage;
		}

		/// <summary>
		/// Gets the selected <see cref="IPresentationImage"/>.
		/// </summary>
		public IPresentationImage PresentationImage 
		{ 
			get { return _presentationImage; } 
		}
	}
}
