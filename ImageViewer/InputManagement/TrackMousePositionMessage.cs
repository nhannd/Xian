#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to handle mouse move messages.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="TileController"/>
	public sealed class TrackMousePositionMessage
	{
		private readonly Point _location;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TrackMousePositionMessage(Point location)
		{
			_location = location;
		}

		/// <summary>
		/// Gets the mouse location.
		/// </summary>
		public Point Location
		{
			get { return _location; }
		}
	}
}
