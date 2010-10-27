#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// An attribute used by <see cref="MouseImageViewerTool"/> to specify it's 
	/// default <see cref="ClearCanvas.ImageViewer.InputManagement.MouseButtonShortcut"/>.
	/// </summary>
	/// <seealso cref="MouseImageViewerTool"/>
	/// <seealso cref="ClearCanvas.ImageViewer.InputManagement.MouseButtonShortcut"/>
	/// <seealso cref="ClearCanvas.ImageViewer.InputManagement.IViewerShortcutManager"/>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MouseToolButtonAttribute : Attribute
	{
		private readonly XMouseButtons _mouseButton;
		private readonly bool _initiallyActive;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseToolButtonAttribute(XMouseButtons mouseButton, bool initiallyActive)
		{
			_mouseButton = mouseButton;
			_initiallyActive = initiallyActive;
		}

		/// <summary>
		/// Gets the mouse button assigned to the <see cref="MouseImageViewerTool"/>.
		/// </summary>
		public XMouseButtons MouseButton
		{
			get { return _mouseButton; }
		}

		/// <summary>
		/// Gets whether or not the tool should be initially active upon opening the viewer.
		/// </summary>
		public bool InitiallyActive
		{
			get { return _initiallyActive; }
		}
	}
}