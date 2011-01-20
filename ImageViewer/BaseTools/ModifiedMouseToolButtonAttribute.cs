#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// An attribute used by <see cref="MouseImageViewerTool"/> to specify it's modified <see cref="MouseButtonShortcut"/>.
	/// </summary>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="MouseImageViewerTool"/>
	/// <seealso cref="IViewerShortcutManager"/>
	/// <seealso cref="DefaultMouseToolButtonAttribute"/>
	[Obsolete("Additional mouse tool button assignments no longer need to be modified.  Use DefaultMouseToolButtonAttribute instead.")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ModifiedMouseToolButtonAttribute : DefaultMouseToolButtonAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ModifiedMouseToolButtonAttribute(XMouseButtons mouseButton, ModifierFlags modifierFlags)
			: base(mouseButton, modifierFlags)
		{
		}
	}
}