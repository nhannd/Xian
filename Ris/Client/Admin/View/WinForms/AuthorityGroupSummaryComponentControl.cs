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
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AuthorityGroupSummaryComponent"/>
    /// </summary>
    public partial class AuthorityGroupSummaryComponentControl : CustomUserControl
    {
        private AuthorityGroupSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorityGroupSummaryComponentControl(AuthorityGroupSummaryComponent component)
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
            _component.LoadAuthorityGroupTable();
        }

        private void _authorityGroups_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedAuthorityGroup();
        }
    }
}
