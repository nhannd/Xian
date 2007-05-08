using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientOrderHistoryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientOrderHistoryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientOrderHistoryComponent class
    /// </summary>
    [AssociateView(typeof(PatientOrderHistoryComponentViewExtensionPoint))]
    public class BiographyOrderHistoryComponent : ApplicationComponent
    {
        private BiographyOrderHistoryTable _orderList;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOrderHistoryComponent()
        {
            _orderList = new BiographyOrderHistoryTable();
        }

        public override void Start()
        {
            base.Start();

            try
            {
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
