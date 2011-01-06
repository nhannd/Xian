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

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DynamicTeComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DynamicTeComponentViewExtensionPoint))]
    public class DynamicTeComponentView : WinFormsView, IApplicationComponentView
    {
        private DynamicTeComponent _component;
        private DynamicTeComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DynamicTeComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DynamicTeComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
