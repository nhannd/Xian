using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    public partial class TechnologistDocumentationComponentControl : ApplicationComponentUserControl
    {
        private readonly TechnologistDocumentationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public TechnologistDocumentationComponentControl(TechnologistDocumentationComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            browserOrderOverview.Url = new Uri(_component.OrderSummaryUrl);
            browserOrderOverview.ObjectForScripting = _component.ScriptObject;

            treeProcedurePlan.Tree = _component.ProcedurePlanTree;
            treeProcedurePlan.MenuModel = _component.ProcedurePlanTreeActionModel;
            treeProcedurePlan.ToolbarModel = _component.ProcedurePlanTreeActionModel;
            _component.ProcedurePlanTreeChanged += OnProcedurePlanChanged;

            tableViewDocumentationDetails.Table = _component.MppsTable;
            tableViewDocumentationDetails.DataBindings.Add("Selection", _component, "SelectedMpps", true, DataSourceUpdateMode.OnPropertyChanged);
            tableViewDocumentationDetails.MenuModel = _component.MppsTableActionModel;
            tableViewDocumentationDetails.ToolbarModel = _component.MppsTableActionModel;
        }

        ~TechnologistDocumentationComponentControl()
        {
            _component.ProcedurePlanTreeChanged -= OnProcedurePlanChanged;
        }

        private void TechnologistDocumentationComponentControl_Load(object sender, EventArgs e)
        {
            treeProcedurePlan.ExpandAll();
            tabControlDetails.SelectTab(tabPageDocumentationDetails);
        }

        private void OnProcedurePlanChanged(object sender, EventArgs e)
        {
            treeProcedurePlan.Tree = _component.ProcedurePlanTree;
            treeProcedurePlan.ExpandAll();
        }

        private void buttonCompleteDocumentationDetails_Click(object sender, EventArgs e)
        {
            _component.OnComplete();
        }
    }
}
