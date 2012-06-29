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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	[ExtensionOf(typeof(ActivityMonitorComponentViewExtensionPoint))]
	public class ActivityMonitorComponentView : WinFormsView, IApplicationComponentView
	{
		private ActivityMonitorComponentControl _control;
		private ActivityMonitorComponent _component;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ActivityMonitorComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ActivityMonitorComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
