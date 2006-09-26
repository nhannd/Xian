using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientSearchResultComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientSearchResultComponentViewExtensionPoint))]
    public class PatientSearchResultComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientSearchResultComponent _component;
        private PatientSearchResultComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientSearchResultComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientSearchResultComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
