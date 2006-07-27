using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(AddressesSummaryComponentViewExtensionPoint))]
    public class AddressesSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private AddressesSummaryComponent _component;
        private AddressesSummaryControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (AddressesSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new AddressesSummaryControl(_component);
                }
                return _control;
            }
        }
    }
}
