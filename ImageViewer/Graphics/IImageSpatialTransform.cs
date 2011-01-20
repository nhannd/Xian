#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a 2D spatial transformation specifically for images.
	/// </summary>
	public interface IImageSpatialTransform : ISpatialTransform
	{
		/// <summary>
		/// Gets or sets a value indicating whether the image should
		/// be scaled to that it fits in the tile that hosts it.
		/// </summary>
		bool ScaleToFit { get; set; }
	}
}
