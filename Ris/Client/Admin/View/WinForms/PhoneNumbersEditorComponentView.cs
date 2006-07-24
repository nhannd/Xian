using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    public class PhoneNumbersEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private PhoneNumbersEditorComponent _component;
        private PhoneNumbersEditorControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PhoneNumbersEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PhoneNumbersEditorControl();
                }
                return _control;
            }
        }
    }
}
