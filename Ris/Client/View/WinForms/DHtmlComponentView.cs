#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    [ExtensionOf(typeof(DHtmlComponentViewExtensionPoint))]
    public class DHtmlComponentView : WinFormsView, IApplicationComponentView
    {
        private DHtmlComponent _component;
        private DHtmlComponentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DHtmlComponent) component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DHtmlComponentControl(_component);
                }
                return _control;
            }
        }

    }
}
