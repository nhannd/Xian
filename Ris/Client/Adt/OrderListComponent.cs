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
        private Table<WorklistItem> _orderList;


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

            IList<WorklistItem> worklistItems = orderEntryService.GetOrdersWorklist();

            _orderList = new Table<WorklistItem>();
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnMRN,
                delegate(WorklistItem item) { return item.Mrn.Format(); }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnName,
                delegate(WorklistItem item) { return item.PatientName.Format(); }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnVisitNumber,
                delegate(WorklistItem item) { return item.VisitNumber.Format(); }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnAccessionNumber,
                delegate(WorklistItem item) { return item.AccessionNumber; }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnDiagnosticService,
                delegate(WorklistItem item) { return item.DiagnosticService; }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnProcedure,
                delegate(WorklistItem item) { return item.Procedure; }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnScheduledStep,
                delegate(WorklistItem item) { return item.ScheduledStep; }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnModality,
                delegate(WorklistItem item) { return item.Modality; }));
            _orderList.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnPriority,
                delegate(WorklistItem item) { return orderPriorities[item.Priority].Value; }));

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
