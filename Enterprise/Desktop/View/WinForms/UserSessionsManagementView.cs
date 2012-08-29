#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto subclasses of <see cref="SummaryComponentBase"/>
    /// </summary>
    [ExtensionOf(typeof(UserSessionManagmentComponentViewExtensionPoint))]
    class UserSessionsManagementView : WinFormsView, IApplicationComponentView
    {
        private UserSessionManagmentComponent _component;
        private UserSessionsManagmentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (UserSessionManagmentComponent) component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                    _control = new UserSessionsManagmentControl(_component);

                return _control;

            }
        }
    }
}
