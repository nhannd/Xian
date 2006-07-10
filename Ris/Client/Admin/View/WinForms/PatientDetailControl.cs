using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PatientDetailControl : UserControl
    {
        private PatientDetailComponent _component;

        public PatientDetailControl(PatientDetailComponent component)
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
