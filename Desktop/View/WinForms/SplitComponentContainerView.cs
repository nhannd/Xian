#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(SplitComponentContainerViewExtensionPoint))]
	public class SplitComponentContainerView : WinFormsView, IApplicationComponentView
	{
		private SplitComponentContainer _component;
		private SplitComponentContainerControl _control;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (SplitComponentContainer)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new SplitComponentContainerControl(_component);
				}
				return _control;
			}
		}
	}
}
