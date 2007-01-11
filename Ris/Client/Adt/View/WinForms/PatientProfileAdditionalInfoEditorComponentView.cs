using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientProfileAdditionalInfoEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientProfileAdditionalInfoEditorComponentViewExtensionPoint))]
    public class PatientProfileAdditionalInfoEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientProfileAdditionalInfoEditorComponent _component;
        private PatientProfileAdditionalInfoEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientProfileAdditionalInfoEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientProfileAdditionalInfoEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
