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
	public partial class UserSummaryComponentControl : ApplicationComponentUserControl
	{
		private UserSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
		public UserSummaryComponentControl(UserSummaryComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _userTableView.ToolbarModel = _component.SummaryTableActionModel;
			_userTableView.MenuModel = _component.SummaryTableActionModel;

            _userTableView.Table = _component.SummaryTable;
            _userTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_id.DataBindings.Add("Value", _component, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
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

		private void _userTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.DoubleClickSelectedItem();
        }

		private void _clearButton_Click(object sender, EventArgs e)
		{
			_id.Value = "";
			_name.Value = "";
			_component.Search();
		}
	}
}