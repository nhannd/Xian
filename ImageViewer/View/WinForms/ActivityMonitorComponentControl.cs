#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class ActivityMonitorComponentControl : ApplicationComponentUserControl
	{
		private ActivityMonitorComponent _component;

		public ActivityMonitorComponentControl(ActivityMonitorComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();
		}
	}
}
