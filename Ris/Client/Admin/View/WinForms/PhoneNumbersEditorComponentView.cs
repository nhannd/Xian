using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    class PhoneNumbersEditorComponentView : WinFormsView, IApplicationComponentView
    {
        PhoneNumbersEditorComponent _component;
        PhoneNumbersEditorControl _control;

        protected PhoneNumbersEditorControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PhoneNumbersEditorControl(_component);
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
            _component = (PhoneNumbersEditorComponent)component;
        }

        #endregion
    }
}
