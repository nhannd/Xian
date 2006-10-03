using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientComponentViewExtensionPoint))]
    public class PatientComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientComponent _component;
        private PatientComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
