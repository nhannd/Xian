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
    /// Provides a Windows Forms user-interface for <see cref="PatientOverviewComponent"/>
    /// </summary>
    public partial class PatientOverviewComponentControl : CustomUserControl
    {
        private PatientOverviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientOverviewComponentControl(PatientOverviewComponent component)
        {
            InitializeComponent();
            _component = component;
            
            _name.DataBindings.Add("Text", _component, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
