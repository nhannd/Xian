#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if DEBUG

using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.Tools;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	[ExtensionOf(typeof (ActionModelsToolComponentViewExtensionPoint))]
	internal sealed class ActionModelsToolComponentView : WinFormsView, IApplicationComponentView
	{
		private ActionModelsToolComponent _component;
		private ActionModelsToolComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ActionModelsToolComponent) component;
		}

		public override object GuiElement
		{
			get { return _control ?? (_control = new ActionModelsToolComponentControl(_component)); }
		}
	}
}

#endif