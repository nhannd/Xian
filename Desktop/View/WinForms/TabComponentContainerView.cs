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
    [ExtensionOf(typeof(TabComponentContainerViewExtensionPoint))]
    public class TabComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private TabComponentContainer _component;
        private TabComponentContainerControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
			Platform.CheckForNullReference(component, "component");
            _component = (TabComponentContainer)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new TabComponentContainerControl(_component);
                }
                return _control;
            }
        }
    }
}
