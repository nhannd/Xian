#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Represents the method that will handle the <see cref="IGraphic.VisualStateChanged"/> event raised
	/// when a property is changed on a graphic, resulting in a change in the graphic's visual state.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="VisualStateChangedEventArgs"/> that contains the event data. </param>
	public delegate void VisualStateChangedEventHandler(object sender, VisualStateChangedEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="IGraphic.VisualStateChanged"/> event. 
	/// </summary>
	public sealed class VisualStateChangedEventArgs : PropertyChangedEventArgs
	{
		/// <summary>
		/// Gets the graphic whose visual state changed.
		/// </summary>
		public readonly IGraphic Graphic;

		/// <summary>
		/// Initializes a new instance of the <see cref="VisualStateChangedEventArgs"/> class. 
		/// </summary>
		/// <param name="graphic">The graphic whose visual state changed.</param>
		/// <param name="propertyName">The name of the property that changed. </param>
		public VisualStateChangedEventArgs(IGraphic graphic, string propertyName)
			: base(propertyName)
		{
			this.Graphic = graphic;
		}
	}
}