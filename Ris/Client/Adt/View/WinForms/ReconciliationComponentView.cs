using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientReconciliationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientReconciliationComponentViewExtensionPoint))]
    public class ReconciliationComponentView : WinFormsView, IApplicationComponentView
    {
        private ReconciliationComponent _component;
        private ReconciliationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ReconciliationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ReconciliationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
