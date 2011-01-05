#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="MergeOrdersComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class MergeOrdersComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// MergeOrdersComponent class.
	/// </summary>
	[AssociateView(typeof(MergeOrdersComponentViewExtensionPoint))]
	public class MergeOrdersComponent : ApplicationComponent
	{
		class MergeOrdersTable : Table<OrderDetail>
		{
			public MergeOrdersTable()
			{
				ITableColumn accesionNumberColumn;
				this.Columns.Add(accesionNumberColumn = new TableColumn<OrderDetail, string>(SR.ColumnAccessionNumber, o => AccessionFormat.Format(o.AccessionNumber), 0.25f));
				this.Columns.Add(new TableColumn<OrderDetail, string>(SR.ColumnImagingService, o => o.DiagnosticService.Name, 0.75f));

				this.Sort(new TableSortParams(accesionNumberColumn, true));
			}
		}

		private readonly List<EntityRef> _orderRefs;
		private readonly MergeOrdersTable _ordersTable;
		private OrderDetail _selectedOrder;
		private OrderDetail _dryRunMergedOrder;

		private TabComponentContainer _mergedOrderViewComponentContainer;
		private ChildComponentHost _mergedOrderPreviewComponentHost;

		private MergedOrderDetailViewComponent _orderPreviewComponent;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;
		private AttachedDocumentPreviewComponent _attachmentSummaryComponent;

		public MergeOrdersComponent(List<EntityRef> orderRefs)
		{
			_orderRefs = orderRefs;
			_ordersTable = new MergeOrdersTable();
		}

		public override void Start()
		{
			_mergedOrderViewComponentContainer = new TabComponentContainer();
			_mergedOrderPreviewComponentHost = new ChildComponentHost(this.Host, _mergedOrderViewComponentContainer);
			_mergedOrderPreviewComponentHost.StartComponent();

			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleOrder, _orderPreviewComponent = new MergedOrderDetailViewComponent()));
			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleAdditionalInfo, _orderAdditionalInfoComponent = new OrderAdditionalInfoComponent(true)));
			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleOrderAttachments, _attachmentSummaryComponent = new AttachedDocumentPreviewComponent(true, AttachedDocumentPreviewComponent.AttachmentMode.Order)));

			// Load form data
			Platform.GetService(
				delegate(IBrowsePatientDataService service)
				{
					var request = new GetDataRequest { GetOrderDetailRequest = new GetOrderDetailRequest() };

					foreach (var orderRef in _orderRefs)
					{
						request.GetOrderDetailRequest.OrderRef = orderRef;
						var response = service.GetData(request);
						_ordersTable.Items.Add(response.GetOrderDetailResponse.Order);
					}
				});

			_ordersTable.Sort();

			// Re-populate orderRef list by sorted accession number
			_orderRefs.Clear();
			_orderRefs.AddRange(CollectionUtils.Map<OrderDetail, EntityRef>(_ordersTable.Items, item => item.OrderRef));

			_selectedOrder = CollectionUtils.FirstElement(_ordersTable.Items);
			DryRunForSelectedOrder();

			base.Start();
		}

		public override void Stop()
		{
			if (_mergedOrderPreviewComponentHost != null)
			{
				_mergedOrderPreviewComponentHost.StopComponent();
				_mergedOrderPreviewComponentHost = null;
			}

			base.Stop();
		}

		#region Presentation Model

		public ITable OrdersTable
		{
			get { return _ordersTable; }
		}

		public ISelection OrdersTableSelection
		{
			get
			{
				return new Selection(_selectedOrder);
			}
			set
			{
				var previousSelection = new Selection(_selectedOrder);
				if (previousSelection.Equals(value))
					return;

				_selectedOrder = (OrderDetail) value.Item;
				DryRunForSelectedOrder();
				NotifyPropertyChanged("SummarySelection");
			}
		}

		public bool AcceptEnabled
		{
			get
			{
				return _ordersTable.Items.Count > 0
					&& _selectedOrder != null
					&& _dryRunMergedOrder != null;
			}
		}

		public ApplicationComponentHost MergedOrderPreviewComponentHost
		{
			get { return _mergedOrderPreviewComponentHost; }
		}

		public void Accept()
		{
			try
			{
				if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(SR.MessageMergeOrders, MessageBoxActions.YesNo))
					return;

				var destinationOrderRef = _selectedOrder.OrderRef;
				var sourceOrderRefs = new List<EntityRef>(_orderRefs);
				sourceOrderRefs.Remove(_selectedOrder.OrderRef);

				Platform.GetService(
					delegate(IOrderEntryService service)
					{
						var request = new MergeOrderRequest(sourceOrderRefs, destinationOrderRef) { DryRun = false };
						service.MergeOrder(request);
					});
				
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionMergeOrdersTool, this.Host.DesktopWindow,
					() => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private void DryRunForSelectedOrder()
		{
			string failureReason;
			MergeOrderDryRun(out _dryRunMergedOrder, out failureReason);
			if (!string.IsNullOrEmpty(failureReason))
				this.Host.ShowMessageBox(failureReason, MessageBoxActions.Ok);

			// Update order preview components
			if (_dryRunMergedOrder == null)
			{
				_orderPreviewComponent.Context = null;
				_orderAdditionalInfoComponent.HealthcareContext = null;
				_orderAdditionalInfoComponent.OrderExtendedProperties = new Dictionary<string, string>();
				_attachmentSummaryComponent.OrderAttachments = new List<OrderAttachmentSummary>();
			}
			else
			{
				_orderPreviewComponent.Context = _dryRunMergedOrder;
				_orderAdditionalInfoComponent.HealthcareContext = _dryRunMergedOrder;
				_orderAdditionalInfoComponent.OrderExtendedProperties = _dryRunMergedOrder.ExtendedProperties;
				_attachmentSummaryComponent.OrderAttachments = _dryRunMergedOrder.Attachments;
			}
		}

		private void MergeOrderDryRun(out OrderDetail mergedOrder, out string failureReason)
		{
			if (_selectedOrder == null)
			{
				failureReason = null;
				mergedOrder = null;
				return;
			}

			var destinationOrderRef = _selectedOrder.OrderRef;
			var sourceOrderRefs = new List<EntityRef>(_orderRefs);
			sourceOrderRefs.Remove(_selectedOrder.OrderRef);

			MergeOrderResponse response = null;
			Platform.GetService(
				delegate(IOrderEntryService service)
				{
					var request = new MergeOrderRequest(sourceOrderRefs, destinationOrderRef) { DryRun = true };
					response = service.MergeOrder(request);
				});

			failureReason = response.DryRunFailureReason;
			mergedOrder = response.DryRunMergedOrder;
		}
	}
}
