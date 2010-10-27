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
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="StackTabComponentContainer"/>
    /// </summary>
    [ExtensionOf(typeof(StackTabComponentContainerViewExtensionPoint))]
    public class StackTabComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private StackTabComponentContainer _component;
        private StackTabComponentContainerControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (StackTabComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new StackTabComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
