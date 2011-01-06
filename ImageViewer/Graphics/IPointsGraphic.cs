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
	/// Defines an <see cref="IVectorGraphic"/> that can be described as an ordered list of independent points.
	/// </summary>
	public interface IPointsGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets the ordered list of points that defines the graphic.
		/// </summary>
		IPointsList Points { get; }
	}
}