using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Common.View.WinForms
{
    [ExtensionOf(typeof(AddressesEditorComponentViewExtensionPoint))]
    class AddressEditorComponentView : WinFormsView, IApplicationComponentView
    {
        AddressEditorComponent _component;
        AddressEditorControl _control;

        protected AddressEditorControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new AddressEditorControl(_component);
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
            _component = (AddressEditorComponent)component;
        }

        #endregion

    }
}
