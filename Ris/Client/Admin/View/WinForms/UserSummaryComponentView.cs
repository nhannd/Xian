using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="UserSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(UserSummaryComponentViewExtensionPoint))]
    public class UserSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private UserSummaryComponent _component;
        private UserSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (UserSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new UserSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
