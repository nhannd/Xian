using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    public partial class TechnologistDocumentationComponentControl : ApplicationComponentUserControl
    {
        private readonly TechnologistDocumentationComponent _component;

        private class CustomValidationRule : IValidationRule
        {
            private readonly TechnologistDocumentationComponentControl _owner;

            public CustomValidationRule(TechnologistDocumentationComponentControl owner)
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
        public TechnologistDocumentationComponentControl(TechnologistDocumentationComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            // add a validation rule that checks the browser for validation errors
            _component.Validation.Add(new CustomValidationRule(this));

            _browser.ObjectForScripting = _component.ScriptObject;
            // Manually bind the Url since DataBinding was updating the page inappropriately
            _browser.Url = _component.DocumentationPage;
            _component.DocumentationPageChanged += delegate { _browser.Url = _component.DocumentationPage; };
            _component.DocumentationDataChanged += delegate { _browser.Document.InvokeScript("setData"); };
            _component.BeforeDocumentationSaved += _component_BeforeDocumentationSaved;

            _component.ValidationVisibleChanged += _component_ValidationVisibleChanged;

            _procedureStepsTable.Table = _component.ProcedureSteps;
            _procedureStepsTable.ToolbarModel = _component.DocumentationActionModel;
        }

        void _component_BeforeDocumentationSaved(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("saveData", new object[] { _component.ValidationVisible });
        }

        void _component_ValidationVisibleChanged(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("showValidation", new object[] { _component.ValidationVisible });
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
