#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
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

			UpdateIcons();
        }

		protected override void OnCurrentUIThemeChanged()
		{
			base.OnCurrentUIThemeChanged();

			UpdateIcons();
		}

		private void UpdateIcons()
		{
			var resourceResolver = new ApplicationThemeResourceResolver(GetType(), false);
			_searchButton.Image = resourceResolver.OpenImage(@"Resources.SearchToolSmall.png");
			_clearButton.Image = resourceResolver.OpenImage(@"Resources.ClearFilterSmall.png");
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