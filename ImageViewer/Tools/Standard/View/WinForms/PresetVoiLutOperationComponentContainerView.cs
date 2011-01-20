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
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof(PresetVoiLutOperationComponentContainerViewExtensionPoint))]
	public class PresetVoiLutOperationComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private PresetVoiLutOperationsComponentContainer _component;
        private PresetVoiLutOperationComponentContainerControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PresetVoiLutOperationsComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PresetVoiLutOperationComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
