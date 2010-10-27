#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a vector based graphic.
	/// </summary>
	public interface IVectorGraphic : IGraphic
	{
		/// <summary>
		/// Gets or sets the colour.
		/// </summary>
		Color Color { get; set; }

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		LineStyle LineStyle { get; set; }
	}
}