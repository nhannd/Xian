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

            Control orderSummary = (Control)_component.OrderSummaryComponentHost.ComponentView.GuiElement;
            orderSummary.Dock = DockStyle.Fill;
            _orderSummaryPanel.Controls.Add(orderSummary);

            Control documentationTabs = (Control)_component.DocumentationHost.ComponentView.GuiElement;
            documentationTabs.Dock = DockStyle.Fill;
            _splitContainerRoot.Panel2.Controls.Add(documentationTabs);

            _treeProcedurePlan.Tree = _component.ProcedurePlanTree;
            _treeProcedurePlan.MenuModel = _component.ProcedurePlanTreeActionModel;
            _treeProcedurePlan.ToolbarModel = _component.ProcedurePlanTreeActionModel;
            _component.ProcedurePlanTreeChanged += OnProcedurePlanChanged;
        }

        ~TechnologistDocumentationComponentControl()
        {
            _component.ProcedurePlanTreeChanged -= OnProcedurePlanChanged;
        }

        private void TechnologistDocumentationComponentControl_Load(object sender, EventArgs e)
        {
            _treeProcedurePlan.ExpandAll();
            //tabControlDetails.SelectTab(tabPageDocumentationDetails);
        }

        private void OnProcedurePlanChanged(object sender, EventArgs e)
        {
            _treeProcedurePlan.Tree = _component.ProcedurePlanTree;
            _treeProcedurePlan.ExpandAll();
        }

        private void _btnSave_Click(object sender, EventArgs e)
        {
            using(new CursorManager(Cursors.WaitCursor))
            {
                _component.SaveData();
            }
        }

        private void _btnComplete_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.CompleteDocumentation();
            }
        }
    }
}
