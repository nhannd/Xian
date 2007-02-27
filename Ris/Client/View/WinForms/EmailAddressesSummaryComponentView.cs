using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="EmailAddressSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(EmailAddressSummaryComponentViewExtensionPoint))]
    public class EmailAddressesSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private EmailAddressesSummaryComponent _component;
        private EmailAddressesSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (EmailAddressesSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new EmailAddressesSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
