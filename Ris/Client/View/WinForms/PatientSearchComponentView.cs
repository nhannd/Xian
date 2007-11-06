using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientSearchComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientSearchComponentViewExtensionPoint))]
    public class PatientSearchComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientSearchComponent _component;
        private PatientSearchComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientSearchComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientSearchComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
