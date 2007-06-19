using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    public partial class TechnologistDocumentationComponentControl : ApplicationComponentUserControl
    {
        private readonly TechnologistDocumentationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public TechnologistDocumentationComponentControl(TechnologistDocumentationComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _browser.ObjectForScripting = _component.ScriptObject;
            _browser.DocumentCompleted += _browser_DocumentCompleted;
            // Manually bind the Url since DataBinding was updating the page inappropriately
            _browser.Url = _component.DocumentationPage;
            _component.DocumentationPageChanged += delegate { _browser.Url = _component.DocumentationPage; };
            _component.DocumentationDataChanged += delegate { _browser.Document.InvokeScript("setData"); };
            _component.BeforeAccept += _component_BeforeAccept;

            _component.ValidationVisibleChanged += _component_ValidationVisibleChanged;

            _procedureStepsTable.Table = _component.ProcedureSteps;
            _procedureStepsTable.ToolbarModel = _component.DocumentationActionModel;
            _procedureStepsTable.DataBindings.Add("Selection", _component, "SelectedProcedureStep", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        void _component_BeforeAccept(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("saveData", new object[] { _component.ValidationVisible });
        }

        void _component_ValidationVisibleChanged(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("showValidation", new object[] { _component.ValidationVisible });
        }

        void _browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
             //_browser.Document.InvokeScript("setData", new object[] { _component.ReportData });
        }
    }
}
