#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	[ExtensionOf(typeof(ColumnPickerComponentViewExtensionPoint))]
	internal class ColumnPickerComponentView : WinFormsView, IApplicationComponentView
	{
		private ColumnPickerComponent _component;
		private ColumnPickerComponentPanel _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ColumnPickerComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ColumnPickerComponentPanel(_component);
				}
				return _control;
			}
		}
	}
}