using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="SettingsManagementComponent"/>
    /// </summary>
    [ExtensionOf(typeof(SettingsManagementComponentViewExtensionPoint))]
    public class SettingsManagementComponentView : WinFormsView, IApplicationComponentView
    {
        private SettingsManagementComponent _component;
        private SettingsManagementComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SettingsManagementComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SettingsManagementComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
