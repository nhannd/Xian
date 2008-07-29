using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
	public partial class LocationSummaryComponentControl : ApplicationComponentUserControl
	{
		private LocationSummaryComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public LocationSummaryComponentControl(LocationSummaryComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_locationTableView.ToolbarModel = _component.SummaryTableActionModel;
			_locationTableView.MenuModel = _component.SummaryTableActionModel;

			_locationTableView.Table = _component.SummaryTable;
			_locationTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_facilityComboBox.DataSource = _component.FacilityChoices;
			_facilityComboBox.DataBindings.Add("Value", _component, "Facility", true, DataSourceUpdateMode.OnPropertyChanged);
			_facilityComboBox.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatFacilityListItem(e.ListItem); };

			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);

			_okButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");
			_cancelButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Search();
			}
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _locationTableView_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.DoubleClickSelectedItem();
		}

		private void _clearButton_Click(object sender, EventArgs e)
		{
			_component.Facility = _component.NullFilter;
			_name.Value = "";
			_component.Search();
		}

		private void _field_Enter(object sender, EventArgs e)
		{
			this.AcceptButton = _searchButton;
		}

		private void _field_Leave(object sender, EventArgs e)
		{
			this.AcceptButton = _okButton;
		}
	}
}
