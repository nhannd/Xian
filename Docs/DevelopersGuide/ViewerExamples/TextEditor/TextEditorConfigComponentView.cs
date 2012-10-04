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

namespace MyPlugin.TextEditor
{
	[ExtensionOf(typeof (TextEditorConfigComponentViewExtensionPoint))]
	public class TextEditorConfigComponentView : WinFormsView, IApplicationComponentView
	{
		private TextEditorConfigComponent _component;
		private TextEditorConfigControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (TextEditorConfigComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new TextEditorConfigControl(_component);
				}
				return _control;
			}
		}
	}
}