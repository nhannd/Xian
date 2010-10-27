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
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof(LinearPresetVoiLutOperationComponentViewExtensionPoint))]
	public class LinearPresetVoiLutOperationComponentView : WinFormsView, IApplicationComponentView
	{
		private LinearPresetVoiLutOperationComponent _component;
		private LinearPresetVoiLutOperationComponentControl _control;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (LinearPresetVoiLutOperationComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new LinearPresetVoiLutOperationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
