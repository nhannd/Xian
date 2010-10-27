#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for events on an <see cref="IGraphic"/>.
	/// </summary>
	public sealed class GraphicEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the <see cref="IGraphic"/> to which the event applies.
		/// </summary>
		public readonly IGraphic Graphic;

		/// <summary>
		/// Constructs a new <see cref="GraphicEventArgs"/>.
		/// </summary>
		/// <param name="graphic">The graphic to which the event applies.</param>
		public GraphicEventArgs(IGraphic graphic)
		{
			this.Graphic = graphic;
		}
	}
}