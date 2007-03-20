using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("view", "global-menus/Orders/View All")]
    [ClickHandler("view", "View")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class OrderListTool : Tool<IDesktopToolContext>
    {
        public void View()
        {
            OrderListComponent component = new OrderListComponent();

            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                component,
                SR.TitleOrders,
                null);
        }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="OrderListComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class OrderListComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// OrderListComponent class
    /// </summary>
    [AssociateView(typeof(OrderListComponentViewExtensionPoint))]
    public class OrderListComponent : ApplicationComponent
    {
        private Table<WorklistQueryResult> _orderList;


        /// <summary>
        /// Constructor
        /// </summary>
        public OrderListComponent()
        {
        }

        public override void Start()
        {
            IOrderEntryService orderEntryService = ApplicationContext.GetService<IOrderEntryService>();
            OrderPriorityEnumTable orderPriorities = orderEntryService.GetOrderPriorityEnumTable();

            IList<WorklistQueryResult> worklistItems = orderEntryService.GetOrdersWorklist(new ModalityProcedureStepSearchCriteria());

            _orderList = new Table<WorklistQueryResult>();
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnMRN,
                delegate(WorklistQueryResult item) { return Format.Custom(item.Mrn); }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnName,
                delegate(WorklistQueryResult item) { return Format.Custom(item.PatientName); }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnVisitNumber,
                delegate(WorklistQueryResult item) { return Format.Custom(item.VisitNumber); }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnAccessionNumber,
                delegate(WorklistQueryResult item) { return item.AccessionNumber; }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnDiagnosticService,
                delegate(WorklistQueryResult item) { return item.DiagnosticService; }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnProcedure,
                delegate(WorklistQueryResult item) { return item.RequestedProcedureName; }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnScheduledStep,
                delegate(WorklistQueryResult item) { return item.ModalityProcedureStepName; }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnModality,
                delegate(WorklistQueryResult item) { return item.ModalityName; }));
            _orderList.Columns.Add(new TableColumn<WorklistQueryResult, string>(SR.ColumnPriority,
                delegate(WorklistQueryResult item) { return orderPriorities[item.Priority].Value; }));

            _orderList.Items.AddRange(worklistItems);

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public ITable Orders
        {
            get { return _orderList; }
        }
    }
}
