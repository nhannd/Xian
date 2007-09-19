using System;
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

            tableViewDocumentationDetails.Table = _component.MppsTable;
            tableViewDocumentationDetails.MenuModel = _component.MppsTableActionModel;
            tableViewDocumentationDetails.ToolbarModel = _component.MppsTableActionModel;
        }
    }
}
