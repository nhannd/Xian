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
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// Specifies a <see cref="ImageViewerTool"/>'s default <see cref="MouseWheelShortcut"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MouseWheelHandlerAttribute : Attribute
	{
		private readonly MouseWheelShortcut _shortcut;

		/// <summary>
		/// Constructor that accepts <see cref="ModifierFlags"/> as input.
		/// </summary>
		/// <param name="modifiers"></param>
		public MouseWheelHandlerAttribute(ModifierFlags modifiers)
		{
			_shortcut = new MouseWheelShortcut(modifiers);
		}

		/// <summary>
		/// Gets the <see cref="ImageViewerTool"/>'s <see cref="MouseWheelShortcut"/>.
		/// </summary>
		public MouseWheelShortcut Shortcut
		{
			get { return _shortcut; }
		}
	}
}
