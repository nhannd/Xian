#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
//                        GetOrdersWorkListResponse response = service.GetOrdersWorkList(new GetOrdersWorkListRequest("UHN"));
//                        _orderList.Items.AddRange(response.Orders);
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
            base.Stop();
        }

        public ITable Orders
        {
            get { return _orderList; }
        }
    }
}
