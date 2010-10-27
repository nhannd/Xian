#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to preview a keyboard button message before it is processed.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="KeyboardButtonShortcut"/>
	/// <seealso cref="TileController"/>
	public sealed class KeyboardButtonDownPreview
	{
		private readonly KeyboardButtonShortcut _buttonShortcut;

		/// <summary>
		/// Constructor.
		/// </summary>
		public KeyboardButtonDownPreview(XKeys keyData)
		{
			_buttonShortcut = new KeyboardButtonShortcut(keyData);
		}

		/// <summary>
		/// Gets the <see cref="KeyboardButtonShortcut"/> to be previewed.
		/// </summary>
		public KeyboardButtonShortcut Shortcut
		{
			get { return _buttonShortcut; }
		}
	}
}
