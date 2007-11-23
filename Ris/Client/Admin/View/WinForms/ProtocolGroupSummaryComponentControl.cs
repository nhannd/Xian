using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProtocolGroupSummaryComponent"/>
    /// </summary>
    public partial class ProtocolGroupSummaryComponentControl : ApplicationComponentUserControl
    {
        private ProtocolGroupSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolGroupSummaryComponentControl(ProtocolGroupSummaryComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _protocolGroupsTable.ToolbarModel = _component.ProtocolGroupListActionModel;
            _protocolGroupsTable.MenuModel = _component.ProtocolGroupListActionModel;
            _protocolGroupsTable.Table = _component.ProtocolGroups;
            _protocolGroupsTable.DataBindings.Add("Selection", _component, "SelectedProtocolGroup", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _protocolGroupsTable_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.EditSelectedProtocolGroup();
        }
    }
}
