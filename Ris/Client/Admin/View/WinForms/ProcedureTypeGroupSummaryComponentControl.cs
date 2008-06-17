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

			_categories.NullItem = _component.NullFilterItem;
			_categories.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatCategory(args.ListItem); };
			_categories.DataBindings.Add("Items", _component, "CategoryChoices", true, DataSourceUpdateMode.Never);
			_categories.DataBindings.Add("CheckedItems", _component, "SelectedCategories", true,
										 DataSourceUpdateMode.OnPropertyChanged);

			_okButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");
			_cancelButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
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
