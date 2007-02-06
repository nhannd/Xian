using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ImportDiagnosticServicesComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ImportDiagnosticServicesComponentViewExtensionPoint))]
    public class ImportDiagnosticServicesComponentView : WinFormsView, IApplicationComponentView
    {
        private ImportDiagnosticServicesComponent _component;
        private ImportDiagnosticServicesComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ImportDiagnosticServicesComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ImportDiagnosticServicesComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
