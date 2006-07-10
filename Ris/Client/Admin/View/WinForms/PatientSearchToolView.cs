using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.View.WinForms;


namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientSearchToolViewExtensionPoint))]
    public class PatientSearchToolView : WinFormsView, IToolView
    {
        private PatientSearchTool _tool;
        private PatientSearchControl _control;

        protected PatientSearchControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientSearchControl();
                    _control.SearchButton.Click += new EventHandler(SearchButton_Click);
                }
                return _control;
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            _tool.Search();
        }

        public override object GuiElement
        {
            get { return this.Control; }
        }
        
        #region IToolView Members

        public void SetTool(ITool tool)
        {
            _tool = (PatientSearchTool)tool;
        }

        #endregion

    }
}
