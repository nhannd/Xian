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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="MppsDocumentationComponent"/>
	/// </summary>
	[ExtensionOf(typeof(MppsDocumentationComponentViewExtensionPoint))]
	public class MppsDocumentationComponentView : WinFormsView, IApplicationComponentView
	{
		private MppsDocumentationComponent _component;
		private MppsDocumentationComponentControl _control;


		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (MppsDocumentationComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new MppsDocumentationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
