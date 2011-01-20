#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="CloseHelperComponent"/>
    /// </summary>
    [ExtensionOf(typeof(CloseHelperComponentViewExtensionPoint))]
    public class CloseHelperComponentView : WinFormsView, IApplicationComponentView
    {
        private CloseHelperComponent _component;
        private CloseHelperComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (CloseHelperComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CloseHelperComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
