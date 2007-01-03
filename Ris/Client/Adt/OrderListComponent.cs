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
        private Table<AcquisitionWorklistItem> _orderList;


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

            IList<AcquisitionWorklistItem> worklistItems = orderEntryService.GetOrdersWorklist(new ScheduledProcedureStepSearchCriteria());

            _orderList = new Table<AcquisitionWorklistItem>();
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnMRN,
                delegate(AcquisitionWorklistItem item) { return item.Mrn.Format(); }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnName,
                delegate(AcquisitionWorklistItem item) { return item.PatientName.Format(); }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnVisitNumber,
                delegate(AcquisitionWorklistItem item) { return item.VisitNumber.Format(); }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnAccessionNumber,
                delegate(AcquisitionWorklistItem item) { return item.AccessionNumber; }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnDiagnosticService,
                delegate(AcquisitionWorklistItem item) { return item.DiagnosticService; }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnProcedure,
                delegate(AcquisitionWorklistItem item) { return item.Procedure; }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnScheduledStep,
                delegate(AcquisitionWorklistItem item) { return item.ScheduledStep; }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnModality,
                delegate(AcquisitionWorklistItem item) { return item.Modality; }));
            _orderList.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnPriority,
                delegate(AcquisitionWorklistItem item) { return orderPriorities[item.Priority].Value; }));

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
