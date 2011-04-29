#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.View.WinForms
{
	public abstract class WinFormsApplicationComponentView<T> : WinFormsView, IApplicationComponentView where T : IApplicationComponent
	{
		protected T Component { get; private set; }
		private object _guiElement;

		public override object GuiElement
		{
			get { return _guiElement ?? (_guiElement = CreateGuiElement()); }
		}

		protected abstract object CreateGuiElement();

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			Component = (T)component;
		}

		#endregion
	}
}
