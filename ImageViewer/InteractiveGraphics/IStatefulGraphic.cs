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

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines a graphic that has state.
	/// </summary>
	public interface IStatefulGraphic : IGraphic
	{
		/// <summary>
		/// Gets or sets the state of the <see cref="IGraphic"/>.
		/// </summary>
		GraphicState State { get; set; }

		/// <summary>
		/// Occurs when the state of the <see cref="IGraphic"/> has changed.
		/// </summary>
		event EventHandler<GraphicStateChangedEventArgs> StateChanged;
	}
}
