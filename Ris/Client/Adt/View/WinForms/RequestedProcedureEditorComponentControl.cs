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
    /// Provides a Windows Forms user-interface for <see cref="RequestedProcedureEditorComponent"/>
    /// </summary>
    public partial class RequestedProcedureEditorComponentControl : ApplicationComponentUserControl
    {
        private RequestedProcedureEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RequestedProcedureEditorComponentControl(RequestedProcedureEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _procedureType.SuggestionProvider = _component.ProcedureTypeSuggestionProvider;
            _procedureType.Format += delegate(object sender, ListControlConvertEventArgs e)
                                         {
                                             e.Value = _component.FormatProcedureType(e.ListItem);
                                         };
            _procedureType.DataBindings.Add("Enabled", _component, "IsProcedureTypeEditable");
            _procedureType.DataBindings.Add("Value", _component, "SelectedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);

            _performingFacility.DataSource = _component.FacilityChoices;
            _performingFacility.DataBindings.Add("Value", _component, "SelectedFacility", true, DataSourceUpdateMode.OnPropertyChanged);
            _performingFacility.DataBindings.Add("Enabled", _component, "IsPerformingFacilityEditable");
            _performingFacility.Format += delegate(object sender, ListControlConvertEventArgs e)
                                         {
                                             e.Value = _component.FormatFacility(e.ListItem);
                                         };

            _laterality.DataSource = _component.LateralityChoices;
            _laterality.DataBindings.Add("Value", _component, "SelectedLaterality", true, DataSourceUpdateMode.OnPropertyChanged);
            
            _scheduledDate.DataBindings.Add("Value", _component, "ScheduledTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _scheduledDate.DataBindings.Add("Enabled", _component, "IsScheduledTimeEditable");
            _scheduledTime.DataBindings.Add("Value", _component, "ScheduledTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _scheduledTime.DataBindings.Add("Enabled", _component, "IsScheduledTimeEditable");

            _portable.DataBindings.Add("Checked", _component, "PortableModality", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
