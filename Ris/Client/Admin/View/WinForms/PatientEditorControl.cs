using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PatientEditorControl : UserControl
    {
        private PatientEditorComponent _component;

        public PatientEditorControl(PatientEditorComponent component)
        {
            InitializeComponent();
            _component = component;
        }

        public Button OkButton
        {
            get { return _okButton; }
        }

        public Button CancelButton
        {
            get { return _cancelButton; }
        }
    }
}
