using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="WorklistSummaryComponent"/>
    /// </summary>
    public partial class WorklistSummaryComponentControl : ApplicationComponentUserControl
    {
        private WorklistSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistSummaryComponentControl(WorklistSummaryComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _worklistTable.MenuModel = _component.WorklistActionModel;
            _worklistTable.ToolbarModel = _component.WorklistActionModel;

            _worklistTable.Table = _component.Worklists;
            _worklistTable.DataBindings.Add("Selection", _component, "SelectedWorklist", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _worklistTable_ItemDoubleClicked(object sender, System.EventArgs e)
        {
            _component.UpdateWorklist();
        }
    }
}
