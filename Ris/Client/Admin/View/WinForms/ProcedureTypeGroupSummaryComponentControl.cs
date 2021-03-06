#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    public partial class ProcedureTypeGroupSummaryComponentControl : ApplicationComponentUserControl
    {
        private readonly ProcedureTypeGroupSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcedureTypeGroupSummaryComponentControl(ProcedureTypeGroupSummaryComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_procedureTypeGroupTableView.ToolbarModel = _component.SummaryTableActionModel;
			_procedureTypeGroupTableView.MenuModel = _component.SummaryTableActionModel;

			_procedureTypeGroupTableView.Table = _component.SummaryTable;
			_procedureTypeGroupTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Value", _component, "SelectedCategory", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatCategory(args.ListItem); };

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

		private void _clearButton_Click(object sender, EventArgs e)
		{
			_component.SelectedCategory = _component.NullFilter;
			_component.Search();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _procedureTypeGroupTableView_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.DoubleClickSelectedItem();
		}
	}
}
