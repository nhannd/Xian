#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyOrderHistoryComponent"/>
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
		private readonly EntityRef _patientRef;
		private readonly OrderListTable _orderList;
		private OrderListItem _selectedOrder;
		private OrderDetail _orderDetail;

		private ChildComponentHost _orderDetailComponentHost;
		private ChildComponentHost _orderVisitComponentHost;
		private ChildComponentHost _orderDocumentComponentHost;
		private ChildComponentHost _orderReportsComponentHost;

		private BiographyOrderDetailViewComponent _orderDetailComponent;
		private VisitDetailViewComponent _visitDetailComponent;
		private MimeDocumentPreviewComponent _orderDocumentComponent;
		private BiographyOrderReportsComponent _orderReportsComponent;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyOrderHistoryComponent(EntityRef patientRef)
		{
			_patientRef = patientRef;
			_orderList = new OrderListTable(3);
		}

		public override void Start()
		{
			Platform.GetService<IBrowsePatientDataService>(
				delegate(IBrowsePatientDataService service)
				{
					GetDataRequest request = new GetDataRequest();
					request.ListOrdersRequest = new ListOrdersRequest(_patientRef, PatientOrdersQueryDetailLevel.Order);
					GetDataResponse response = service.GetData(request);

					_orderList.Items.AddRange(response.ListOrdersResponse.Orders);
				});

			_orderDetailComponent = new BiographyOrderDetailViewComponent();
			_orderDetailComponentHost = new ChildComponentHost(this.Host, _orderDetailComponent);
			_orderDetailComponentHost.StartComponent();

			_visitDetailComponent = new VisitDetailViewComponent();
			_orderVisitComponentHost = new ChildComponentHost(this.Host, _visitDetailComponent);
			_orderVisitComponentHost.StartComponent();

			_orderReportsComponent = new BiographyOrderReportsComponent();
			_orderReportsComponentHost = new ChildComponentHost(this.Host, _orderReportsComponent);
			_orderReportsComponentHost.StartComponent();

			_orderDocumentComponent = new MimeDocumentPreviewComponent(true, true, MimeDocumentPreviewComponent.AttachmentMode.Order);
			_orderDocumentComponentHost = new ChildComponentHost(this.Host, _orderDocumentComponent);
			_orderDocumentComponentHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			_orderDetailComponentHost.StopComponent();
			_orderVisitComponentHost.StopComponent();
			_orderReportsComponentHost.StopComponent();
			_orderDocumentComponentHost.StopComponent();

			base.Stop();
		}

		#region Presentation Model

		public ITable Orders
		{
			get { return _orderList; }
		}

		public ISelection SelectedOrder
		{
			get { return new Selection(_selectedOrder); }
			set
			{
				OrderListItem newSelection = (OrderListItem)value.Item;
				if (_selectedOrder != newSelection)
				{
					_selectedOrder = newSelection;
					OrderSelectionChanged();
				}
			}
		}

		public ApplicationComponentHost OrderDetailComponentHost
		{
			get { return _orderDetailComponentHost; }
		}

		public ApplicationComponentHost OrderVisitComponentHost
		{
			get { return _orderVisitComponentHost; }
		}

		public ApplicationComponentHost OrderReportsComponentHost
		{
			get { return _orderReportsComponentHost; }
		}

		public ApplicationComponentHost OrderDocumentComponentHost
		{
			get { return _orderDocumentComponentHost; }
		}

		#endregion

		private void OrderSelectionChanged()
		{
			try
			{
				if (_selectedOrder != null)
				{
					Platform.GetService<IBrowsePatientDataService>(
						delegate(IBrowsePatientDataService service)
						{
							GetDataRequest request = new GetDataRequest();
							request.GetOrderDetailRequest = new GetOrderDetailRequest(_selectedOrder.OrderRef, true, true, false, false, true);
							GetDataResponse response = service.GetData(request);

							_orderDetail = response.GetOrderDetailResponse.Order;
						});
				}

				UpdatePages();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}

			NotifyAllPropertiesChanged();
		}

		private void UpdatePages()
		{
			if (_selectedOrder == null)
			{
				_orderDetailComponent.Context = null;
				_visitDetailComponent.Context = null;
				_orderReportsComponent.Context = null;
				_orderDocumentComponent.OrderAttachments = new List<OrderAttachmentSummary>();
			}
			else
			{
				_orderDetailComponent.Context = new OrderDetailViewComponent.OrderContext(_selectedOrder.OrderRef);
				_visitDetailComponent.Context = new VisitDetailViewComponent.VisitContext(_selectedOrder.VisitRef);
				_orderReportsComponent.Context = new BiographyOrderReportsComponent.ReportsContext(_selectedOrder.OrderRef, _orderDetail.PatientRef);
				_orderDocumentComponent.OrderAttachments = _orderDetail == null ? new List<OrderAttachmentSummary>() : _orderDetail.Attachments;
			}

		}
	}
}
