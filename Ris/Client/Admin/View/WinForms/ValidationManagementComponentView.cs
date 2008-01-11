using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ValidationManagementComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ValidationManagementComponentViewExtensionPoint))]
    public class ValidationManagementComponentView : WinFormsView, IApplicationComponentView
    {
        private ValidationManagementComponent _component;
        private ValidationManagementComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ValidationManagementComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ValidationManagementComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
