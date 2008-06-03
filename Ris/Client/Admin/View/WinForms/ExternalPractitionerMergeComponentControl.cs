using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerMergeComponent"/>
	/// </summary>
	public partial class ExternalPractitionerMergeComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerMergeComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerMergeComponentControl(ExternalPractitionerMergeComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_duplicates.Table = _component.SummaryTable;
			_duplicates.MenuModel = _component.SummaryTableActionModel;
			_duplicates.ToolbarModel = _component.SummaryTableActionModel;
			_duplicates.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
			_givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
			_middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);
			_licenseNumber.DataBindings.Add("Value", _component, "LicenseNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_billingNumber.DataBindings.Add("Value", _component, "BillingNumber", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
