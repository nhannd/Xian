#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	[ExtensionOf(typeof (ListFilterMenuActionViewExtensionPoint))]
	public class ListFilterMenuActionView : WinFormsActionView
	{
		private object _guiElement;
		
		public ListFilterMenuActionView()
		{}

		public override object GuiElement
		{
			get
			{
				if (_guiElement == null)
				{
					_guiElement = new ClickableToolStripControlHost(
						new ListFilterControl((ListFilterMenuAction)base.Context.Action));
				}

				return _guiElement;
			}
		}
	}
}