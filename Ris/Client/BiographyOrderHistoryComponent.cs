#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom pages to be displayed in the biography order history component.
	/// </summary>
	public interface IBiographyOrderHistoryPageProvider : IExtensionPageProvider<IBiographyOrderHistoryPage, IBiographyOrderHistoryContext>
	{
	}

	/// <summary>
	/// Defines an interface to a custom reporting page.
	/// </summary>
	public interface IBiographyOrderHistoryPage : IExtensionPage
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom page with access to the order context.
	/// </summary>
	public interface IBiographyOrderHistoryContext
	{
		/// <summary>
		/// Gets the reporting worklist item.
		/// </summary>
		OrderListItem OrderListItem { get; }

		/// <summary>
		/// Occurs to indicate that the <see cref="OrderListItem"/> property has changed,
		/// meaning the entire order context is now focused on a different order.
		/// </summary>
		event EventHandler OrderListItemChanged;

		/// <summary>
		/// Gets the order detail associated with the report.
		/// </summary>
		OrderDetail Order { get; }
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the reporting component.
	/// </summary>
	[ExtensionPoint]
	public class BiographyOrderHistoryPageProviderExtensionPoint : ExtensionPoint<IBiographyOrderHistoryPageProvider>
	{
	}

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
		private class BiographyOrderHistoryContext : IBiographyOrderHistoryContext
		{
			private readonly BiographyOrderHistoryComponent _component;

			public BiographyOrderHistoryContext(BiographyOrderHistoryComponent component)
			{
				_component = component;
			}

			#region IBiographyOrderHistoryContext Members

			public OrderListItem OrderListItem
			{
				get { return _component._selectedOrder; }
			}

			public event EventHandler OrderListItemChanged
			{
				add { _component._orderLlistItemChanged += value; }
				remove { _component._orderLlistItemChanged -= value; }
			}

			public OrderDetail Order
			{
				get { return _component._orderDetail; }
			}

			#endregion
		}

		private readonly EntityRef _patientRef;
		private readonly OrderListTable _orderList;
		private OrderListItem _selectedOrder;
		private OrderDetail _orderDetail;

		private ChildComponentHost _rightHandComponentContainerHost;
		private TabComponentContainer _rightHandComponentContainer;

		private BiographyOrderDetailViewComponent _orderDetailComponent;
		private VisitDetailViewComponent _visitDetailComponent;
		private MimeDocumentPreviewComponent _orderDocumentComponent;
		private BiographyOrderReportsComponent _orderReportsComponent;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;

		private List<IBiographyOrderHistoryPage> _extensionPages;
		private event EventHandler _orderLlistItemChanged;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyOrderHistoryComponent(EntityRef patientRef)
		{
			_patientRef = patientRef;
			_orderList = new OrderListTable();
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
			_visitDetailComponent = new BiographyVisitDetailViewComponent();
			_orderReportsComponent = new BiographyOrderReportsComponent();
			_orderDocumentComponent = new MimeDocumentPreviewComponent(true, true, MimeDocumentPreviewComponent.AttachmentMode.Order);
			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent(true);

			_rightHandComponentContainer = new TabComponentContainer();
			_rightHandComponentContainer.Pages.Add(new TabPage("Order Details", _orderDetailComponent));
			_rightHandComponentContainer.Pages.Add(new TabPage("Visit Details", _visitDetailComponent));
			_rightHandComponentContainer.Pages.Add(new TabPage("Additional Info", _orderAdditionalInfoComponent));
			_rightHandComponentContainer.Pages.Add(new TabPage("Reports", _orderReportsComponent));
			_rightHandComponentContainer.Pages.Add(new TabPage("Attachments", _orderDocumentComponent));

			// instantiate all extension pages
			_extensionPages = new List<IBiographyOrderHistoryPage>();
			foreach (IBiographyOrderHistoryPageProvider pageProvider in new BiographyOrderHistoryPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new BiographyOrderHistoryContext(this)));
			}

			// add extension pages to container and set initial context
			// the container will start those components if the user goes to that page
			foreach (IBiographyOrderHistoryPage page in _extensionPages)
			{
				_rightHandComponentContainer.Pages.Add(new TabPage(page.Path.LocalizedPath, page.GetComponent()));
			}

			_rightHandComponentContainerHost = new ChildComponentHost(this.Host, _rightHandComponentContainer);
			_rightHandComponentContainerHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_rightHandComponentContainerHost != null)
			{
				_rightHandComponentContainerHost.StopComponent();
				_rightHandComponentContainerHost = null;
			}

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

		public ApplicationComponentHost RightHandComponentContainerHost
		{
			get { return _rightHandComponentContainerHost; }
		}

		public string BannerText
		{
			get
			{
				return _selectedOrder == null ? null :
					string.Format("{0} - {1}", AccessionFormat.Format(_selectedOrder.AccessionNumber),
					_selectedOrder.DiagnosticService.Name);
			}
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
							request.GetOrderDetailRequest = new GetOrderDetailRequest(_selectedOrder.OrderRef, true, true, false, false, true, false);
							request.GetOrderDetailRequest.IncludeExtendedProperties = true;
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
				_orderAdditionalInfoComponent.OrderExtendedProperties = new Dictionary<string, string>();
				_orderAdditionalInfoComponent.HealthcareContext = null;
			}
			else
			{
				_orderDetailComponent.Context = new OrderDetailViewComponent.OrderContext(_selectedOrder.OrderRef);
				_visitDetailComponent.Context = new VisitDetailViewComponent.VisitContext(_selectedOrder.VisitRef);
				_orderReportsComponent.Context = new BiographyOrderReportsComponent.ReportsContext(_selectedOrder.OrderRef, _orderDetail.PatientRef);
				_orderDocumentComponent.OrderAttachments = _orderDetail == null ? new List<OrderAttachmentSummary>() : _orderDetail.Attachments;
				_orderAdditionalInfoComponent.OrderExtendedProperties = _orderDetail.ExtendedProperties;
				_orderAdditionalInfoComponent.HealthcareContext = _selectedOrder;
			}

			EventsHelper.Fire(_orderLlistItemChanged, this, EventArgs.Empty);
		}
	}
}
