#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a 2D spatial transformation.
	/// </summary>
	public interface ISpatialTransform : IMemorable
	{
		/// <summary>
		/// Gets or sets a value indicating that the object is flipped vertically
		/// (i.e. mirrored on the x-axis).
		/// </summary>
		bool FlipX { get; set; }

		/// <summary>
		/// Gets or sets a value indicating that the object is flipped horizontally
		/// (i.e. mirrored on the y-axis).
		/// </summary>
		bool FlipY { get; set; }

		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		int RotationXY { get; set; }

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		float Scale { get; set; }
		
		/// <summary>
		/// Gets or sets the translation in the x-direction.
		/// </summary>
		float TranslationX { get; set; }

		/// <summary>
		/// Gets or sets the translation in the y-direction.
		/// </summary>
		float TranslationY { get; set; }

		/// <summary>
		/// Gets or sets the center of rotation.
		/// </summary>
		/// <remarks>
		/// The point should be specified in terms of the coordinate system
		/// of the parent graphic, i.e. source coordinates.
		/// </remarks>
		PointF CenterOfRotationXY { get; set; }

		/// <summary>
		/// Gets the cumulative scale.
		/// </summary>
		/// <remarks>
		/// Gets the scale relative to the root of the scene graph.
		/// </remarks>
		float CumulativeScale { get; }
	}
}
