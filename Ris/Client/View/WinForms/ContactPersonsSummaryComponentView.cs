using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Common.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ContactPersonsSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ContactPersonsSummaryComponentViewExtensionPoint))]
    public class ContactPersonsSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private ContactPersonsSummaryComponent _component;
        private ContactPersonsSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ContactPersonsSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ContactPersonsSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
