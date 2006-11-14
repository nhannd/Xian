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
    /// Provides a Windows Forms user-interface for <see cref="VisitEditorDetailsComponent"/>
    /// </summary>
    public partial class VisitDetailsEditorComponentControl : CustomUserControl
    {
        private VisitDetailsEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitDetailsEditorComponentControl(VisitDetailsEditorComponent component)
        {
            InitializeComponent();

            _component = component;

            _visitNumber.DataBindings.Add("Value", _component, "VisitNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _visitNumberAssigningAuthority.DataSource = _component.VisitNumberAssigningAuthorityChoices;
            _visitNumberAssigningAuthority.DataBindings.Add("Value", _component, "VisitNumberAssigningAuthority", true, DataSourceUpdateMode.OnPropertyChanged);

            _admitDateTime.DataBindings.Add("Value", _component, "AdmitDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _dischargeDateTime.DataBindings.Add("Value", _component, "DischargeDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _dischargeDisposition.DataBindings.Add("Value", _component, "DischargeDisposition", true, DataSourceUpdateMode.OnPropertyChanged);

            _vip.DataBindings.Add("Checked", _component, "Vip", true, DataSourceUpdateMode.OnPropertyChanged);
            _preadmitNumber.DataBindings.Add("Value", _component, "PreAdmitNumber", true, DataSourceUpdateMode.OnPropertyChanged);

            _patientClass.DataSource = _component.PatientClassChoices;
            _patientClass.DataBindings.Add("Value", _component, "PatientClass", true, DataSourceUpdateMode.OnPropertyChanged);

            _patientType.DataSource = _component.PatientTypeChoices;
            _patientType.DataBindings.Add("Value", _component, "PatientType", true, DataSourceUpdateMode.OnPropertyChanged);

            _admissionType.DataSource = _component.AdmissionTypeChoices;
            _admissionType.DataBindings.Add("Value", _component, "AdmissionType", true, DataSourceUpdateMode.OnPropertyChanged);

            _visitStatus.DataSource = _component.VisitStatusChoices;
            _visitStatus.DataBindings.Add("Value", _component, "VisitStatus", true, DataSourceUpdateMode.OnPropertyChanged);

            _ambulatoryStatus.DataSource = _component.AmbulatoryStatusChoices;
            _ambulatoryStatus.DataBindings.Add("Value", _component, "AmbulatoryStatus", true, DataSourceUpdateMode.OnPropertyChanged);

            // TODO add .NET databindings to _component
        }
    }
}
