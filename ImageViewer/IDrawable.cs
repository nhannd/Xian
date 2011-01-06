#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides drawing functionality.
	/// </summary>
	public interface IDrawable
	{
		/// <summary>
		/// Fires just before the object is actually drawn/rendered.
		/// </summary>
		event EventHandler Drawing;

		/// <summary>
		/// Draw the object.
		/// </summary>
		void Draw();
	}
}
