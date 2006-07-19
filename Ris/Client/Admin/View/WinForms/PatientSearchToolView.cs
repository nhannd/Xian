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
                    _control = new PatientSearchControl(_tool);
                }
                return _control;
            }
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
