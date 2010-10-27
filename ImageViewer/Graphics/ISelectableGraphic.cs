#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides selection support.
	/// </summary>
	public interface ISelectableGraphic : IGraphic
	{
		/// <summary>
		/// Gets or set a value indicating whether the <see cref="IGraphic"/> is selected.
		/// </summary>
		bool Selected { get; set; }
	}
}
