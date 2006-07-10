using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientDetailComponentViewExtensionPoint))]
    public class PatientDetailComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientDetailControl _control;
        private PatientDetailComponent _component;

        public PatientDetailComponentView()
        {
        }

        protected PatientDetailControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientDetailControl(_component);
                    _control.OkButton.Click += new EventHandler(OkButton_Click);
                    _control.CancelButton.Click += new EventHandler(CancelButton_Click);
                }
                return _control;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            _component.Complete();
        }

        public override object GuiElement
        {
            get { return this.Control; }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientDetailComponent)component;
        }

        #endregion
    }
}
