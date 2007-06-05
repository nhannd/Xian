using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PerformedProcedureComponent"/>
    /// </summary>
    public partial class PerformedProcedureComponentControl : ApplicationComponentUserControl
    {
        private PerformedProcedureComponent _component;

        private class CustomValidationRule : IValidationRule
        {
            private PerformedProcedureComponentControl _owner;
            
            public CustomValidationRule(PerformedProcedureComponentControl owner)
            {
                _owner = owner;
            }

            #region IValidationRule Members

            public string PropertyName
            {
                get { return "DUMMY_PROPERTY"; }
            }

            public ValidationResult GetResult(IApplicationComponent component)
            {
                object result = _owner._browser.Document.InvokeScript("hasValidationErrors");

                // if result == null, the hasValidationErrors method is not implemented by the page
                // in this case, assume there are no errors
                bool hasErrors = (result == null) ? false : (bool)result;
                return new ValidationResult(!hasErrors, "");
            }

            #endregion
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponentControl(PerformedProcedureComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // add a validation rule that checks the browser for validation errors
            _component.Validation.Add(new CustomValidationRule(this));

            _browser.ObjectForScripting = _component.ScriptObject;
            _browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_procedureReport_DocumentCompleted);

            if (!string.IsNullOrEmpty(_component.ReportPageUrl))
            {
                _browser.Url = new Uri(_component.ReportPageUrl);
            }

            _component.ValidationVisibleChanged += new EventHandler(_component_ValidationVisibleChanged);
            _component.BeforeAccept += new EventHandler(_component_BeforeAccept);

        }

        void _component_BeforeAccept(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("saveData", new object[] { _component.ValidationVisible });
        }

        void _component_ValidationVisibleChanged(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("showValidation", new object[] { _component.ValidationVisible });
        }

        void _procedureReport_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // _browser.Document.InvokeScript("setData", new object[] { _component.ReportData });
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Accept();
            }
        }

        private void _validationButton_Click(object sender, EventArgs e)
        {
            _component.Validate();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
