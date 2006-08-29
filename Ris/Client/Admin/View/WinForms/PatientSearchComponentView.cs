using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.View.WinForms;


namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientSearchComponentViewExtensionPoint))]
    public class PatientSearchComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientSearchComponent _component;
        private PatientSearchControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientSearchControl(_component);
                }
                return _control;
            }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientSearchComponent)component;
        }

        #endregion

    }
}
