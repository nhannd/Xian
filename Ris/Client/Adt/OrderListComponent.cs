using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
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
        private Table<ModalityWorklistQueryResult> _orderList;


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

            IList<ModalityWorklistQueryResult> worklistItems = orderEntryService.GetOrdersWorklist(new ModalityProcedureStepSearchCriteria());

            _orderList = new Table<ModalityWorklistQueryResult>();
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnMRN,
                delegate(ModalityWorklistQueryResult item) { return item.Mrn.Format(); }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnName,
                delegate(ModalityWorklistQueryResult item) { return item.PatientName.Format(); }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnVisitNumber,
                delegate(ModalityWorklistQueryResult item) { return item.VisitNumber.Format(); }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnAccessionNumber,
                delegate(ModalityWorklistQueryResult item) { return item.AccessionNumber; }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnDiagnosticService,
                delegate(ModalityWorklistQueryResult item) { return item.DiagnosticService; }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnProcedure,
                delegate(ModalityWorklistQueryResult item) { return item.RequestedProcedureName; }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnScheduledStep,
                delegate(ModalityWorklistQueryResult item) { return item.ModalityProcedureStepName; }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnModality,
                delegate(ModalityWorklistQueryResult item) { return item.ModalityName; }));
            _orderList.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnPriority,
                delegate(ModalityWorklistQueryResult item) { return orderPriorities[item.Priority].Value; }));

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
