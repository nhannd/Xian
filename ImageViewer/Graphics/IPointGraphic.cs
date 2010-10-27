#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that can be defined as a single point.
	/// </summary>
	public interface IPointGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets or sets the point associated with this graphic.
		/// </summary>
		PointF Point { get; set; }

		/// <summary>
		/// Occurs when the value of <see cref="Point"/> changes.
		/// </summary>
		event EventHandler PointChanged;
	}
}