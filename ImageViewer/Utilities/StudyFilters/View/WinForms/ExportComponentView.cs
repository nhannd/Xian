#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Export;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	[ExtensionOf(typeof (ExportComponentViewExtensionPoint))]
	internal class ExportComponentView : WinFormsView, IApplicationComponentView
	{
		private ExportComponent _component;
		private ExportComponentPanel _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ExportComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ExportComponentPanel(_component);
				}
				return _control;
			}
		}
	}
}