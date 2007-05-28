using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PerformedProcedureComponent"/>
    /// </summary>
    public partial class PerformedProcedureComponentControl : ApplicationComponentUserControl
    {
        private PerformedProcedureComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponentControl(PerformedProcedureComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _procedureReport.ObjectForScripting = _component.ScriptObject;
            _procedureReport.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_procedureReport_DocumentCompleted);
            // TODO add .NET databindings to _component

            if (!string.IsNullOrEmpty(_component.ReportPageUrl))
            {

                _procedureReport.Url = new Uri(_component.ReportPageUrl);
            }

        }

        void _procedureReport_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string data = _component.ReportData;
            if (!string.IsNullOrEmpty(data))
            {
                _procedureReport.Document.InvokeScript("setData", new object[] { data });
            }
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            _component.ReportData = (string)_procedureReport.Document.InvokeScript("getData");
            _component.Close();
        }
    }
}
