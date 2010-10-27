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
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	[ExtensionOf(typeof(ServerTreeConfigurationComponentViewExtensionPoint))]
	public class ServerTreeConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private ServerTreeConfigurationComponent _component;
		private ServerTreeConfigurationComponentControl _control;

		#region IApplicationComponentView Members

		/// <summary>
		/// Called by the host to assign this view to a component.
		/// </summary>
		public void SetComponent(IApplicationComponent component)
		{
			_component = (ServerTreeConfigurationComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ServerTreeConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}