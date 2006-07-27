using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(AddressesEditorComponentViewExtensionPoint))]
    class AddressesEditorComponentView : WinFormsView, IApplicationComponentView
    {
        AddressesEditorComponent _component;
        AddressesEditorControl _control;

        protected AddressesEditorControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new AddressesEditorControl(_component);
                }
                return _control;
            }
        }

        public override object GuiElement
        {
            get { return this.Control; }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (AddressesEditorComponent)component;
        }

        #endregion

    }
}
