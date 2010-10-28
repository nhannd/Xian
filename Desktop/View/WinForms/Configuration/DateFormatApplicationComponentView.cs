#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.Standard;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="DicomConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionOf(typeof(DateFormatApplicationComponentViewExtensionPoint))]
	public class DateFormatApplicationComponentView : WinFormsView, IApplicationComponentView
	{
		private DateFormatApplicationComponent _component;
		private DateFormatApplicationComponentControl _control;


		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (DateFormatApplicationComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new DateFormatApplicationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}