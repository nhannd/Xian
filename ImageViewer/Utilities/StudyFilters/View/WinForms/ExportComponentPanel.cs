using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Export;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	public partial class ExportComponentPanel : UserControl
	{
		private readonly ExportComponent _component;

		public ExportComponentPanel(ExportComponent component)
		{
			InitializeComponent();

			_component = component;

			_patientId.DataBindings.Add("Value", _component, "PatientId", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientsName.DataBindings.Add("Value", _component, "PatientsName", true, DataSourceUpdateMode.OnPropertyChanged);
			_dateOfBirth.DataBindings.Add("Value", _component, "PatientsDateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyId.DataBindings.Add("Value", _component, "StudyId", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Value", _component, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDate.DataBindings.Add("Value", _component, "StudyDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_outputPath.DataBindings.Add("Text", _component, "OutputPath", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _browse_Click(object sender, EventArgs e)
		{
			_component.ShowOutputPathDialog();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Anonymize();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Abort();
		}
	}
}
