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
	/// A message object created by the view layer to notify a controlling object 
	/// (e.g. <see cref="TileController"/>) that capture should be released.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="TileController"/>
	public class ReleaseCaptureMessage
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ReleaseCaptureMessage()
		{ 
		}
	}
}
