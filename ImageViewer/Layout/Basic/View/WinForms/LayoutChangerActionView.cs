#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// A WinForms view of the <see cref="LayoutChangerAction"/>.
	/// </summary>
	[ExtensionOf(typeof (LayoutChangerActionViewExtensionPoint))]
	public class LayoutChangerActionView : WinFormsActionView
	{
		private object _guiElement;
		
		public LayoutChangerActionView()
		{}

		public override object GuiElement
		{
			get
			{
				if (_guiElement == null)
					_guiElement = new LayoutChangerToolStripItem((LayoutChangerAction)base.Context.Action);

				return _guiElement;
			}
		}
	}
}