using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(AddressesEditorComponentViewExtensionPoint))]
    public class AddressesEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private AddressesEditorComponent _component;
        private AddressesEditorControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (AddressesEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new AddressesEditorControl();
                }
                return _control;
            }
        }
    }
}
