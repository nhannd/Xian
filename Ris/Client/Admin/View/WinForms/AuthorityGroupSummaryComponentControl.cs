using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AuthorityGroupSummaryComponent"/>
    /// </summary>
    public partial class AuthorityGroupSummaryComponentControl : ApplicationComponentUserControl
    {
        private readonly AuthorityGroupSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorityGroupSummaryComponentControl(AuthorityGroupSummaryComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _authorityGroups.ToolbarModel = _component.AuthorityGroupListActionModel;
            _authorityGroups.MenuModel = _component.AuthorityGroupListActionModel;

            _authorityGroups.Table = _component.AuthorityGroups;
            _authorityGroups.DataBindings.Add("Selection", _component, "SelectedAuthorityGroup", true, DataSourceUpdateMode.OnPropertyChanged);

        }

        private void _authorityGroups_Load(object sender, EventArgs e)
        {
        }

        private void _authorityGroups_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedAuthorityGroup();
        }
    }
}
