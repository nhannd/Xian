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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="CancelOrderComponent"/>
	/// </summary>
	[ExtensionOf(typeof(CancelOrderComponentViewExtensionPoint))]
	public class CancelOrderComponentView : WinFormsView, IApplicationComponentView
	{
		private CancelOrderComponent _component;
		private CancelOrderComponentControl _control;


		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (CancelOrderComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new CancelOrderComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
