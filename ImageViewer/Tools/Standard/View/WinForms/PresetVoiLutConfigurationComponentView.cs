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
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PresetVoiLutConfigurationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PresetVoiLutConfigurationComponentViewExtensionPoint))]
    public class PresetVoiLutConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private PresetVoiLutConfigurationComponent _component;
        private PresetVoiLutConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PresetVoiLutConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PresetVoiLutConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
