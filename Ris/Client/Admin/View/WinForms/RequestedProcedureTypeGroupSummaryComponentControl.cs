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
    /// Provides a Windows Forms user-interface for <see cref="RequestedProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    public partial class RequestedProcedureTypeGroupSummaryComponentControl : ApplicationComponentUserControl
    {
        private readonly RequestedProcedureTypeGroupSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RequestedProcedureTypeGroupSummaryComponentControl(RequestedProcedureTypeGroupSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _requestedProcedureTypeGroupTable.MenuModel = _component.RequestedProcedureTypeGroupListActionModel;
            _requestedProcedureTypeGroupTable.ToolbarModel = _component.RequestedProcedureTypeGroupListActionModel;

            _requestedProcedureTypeGroupTable.Table = _component.RequestedProcedureTypeGroups;
            _requestedProcedureTypeGroupTable.DataBindings.Add("Selection", _component, "SelectedRequestedProcedureTypeGroup", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _requestedProcedureTypeGroupTable_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateRequestedProcedureTypeGroup();
        }
    }
}
