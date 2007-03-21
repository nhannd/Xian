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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

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
        private OrderSummaryTable _orderList;


        /// <summary>
        /// Constructor
        /// </summary>
        public OrderListComponent()
        {
        }

        public override void Start()
        {
            try
            {
                _orderList = new OrderSummaryTable();

                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        GetOrdersWorkListResponse response = service.GetOrdersWorkList(new GetOrdersWorkListRequest("UHN"));
                        _orderList.Items.AddRange(response.Orders);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

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
