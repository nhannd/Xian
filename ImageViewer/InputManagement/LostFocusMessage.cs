#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Message object created by the view layer to allow a controlling object <see cref="TileController"/>
	/// to be notified that the <see cref="Tile"/> has lost input focus.
	/// </summary>
	public sealed class LostFocusMessage
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public LostFocusMessage()
		{
		}
	}
}