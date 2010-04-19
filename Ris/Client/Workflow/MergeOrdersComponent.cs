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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

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
		private readonly EntityRef _order1Ref;
		private readonly EntityRef _order2Ref;
		private OrderDetail _order1;
		private OrderDetail _order2;
		private bool _mergingRight =  true;  // Merging right meaning order1 --> order2

		private TabComponentContainer _mergedOrderViewComponentContainer;
		private ChildComponentHost _mergedOrderPreviewComponentHost;

		private BiographyOrderDetailViewComponent _orderPreviewComponent;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;
		private AttachedDocumentPreviewComponent _attachmentSummaryComponent;

		public MergeOrdersComponent(EntityRef order1Ref, EntityRef order2Ref)
		{
			_order1Ref = order1Ref;
			_order2Ref = order2Ref;
		}

		public override void Start()
		{
			_mergedOrderViewComponentContainer = new TabComponentContainer();
			_mergedOrderPreviewComponentHost = new ChildComponentHost(this.Host, _mergedOrderViewComponentContainer);
			_mergedOrderPreviewComponentHost.StartComponent();

			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleOrder, _orderPreviewComponent = new BiographyOrderDetailViewComponent()));
			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleAdditionalInfo, _orderAdditionalInfoComponent = new OrderAdditionalInfoComponent(true)));
			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleOrderAttachments, _attachmentSummaryComponent = new AttachedDocumentPreviewComponent(true, AttachedDocumentPreviewComponent.AttachmentMode.Order)));

			// Load form data
			Platform.GetService(
				delegate(IBrowsePatientDataService service)
				{
					var request = new GetDataRequest { GetOrderDetailRequest = new GetOrderDetailRequest() };

					request.GetOrderDetailRequest.OrderRef = _order1Ref;
					var response1 = service.GetData(request);
					_order1 = response1.GetOrderDetailResponse.Order;

					request.GetOrderDetailRequest.OrderRef = _order2Ref;
					var response2 = service.GetData(request);
					_order2 = response2.GetOrderDetailResponse.Order;
				});

			var dryRunMergedOrder = SubmitMergeRequest(true);
			ShowMergedOrder(dryRunMergedOrder);

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

		public string Order1AccessionNumber
		{
			get { return AccessionFormat.Format(_order1.AccessionNumber); }
		}

		public string Order1DiagnosticServiceName
		{
			get { return _order1.DiagnosticService.Name; }
		}

		public string Order2AccessionNumber
		{
			get { return AccessionFormat.Format(_order2.AccessionNumber); }
		}

		public string Order2DiagnosticServiceName
		{
			get { return _order2.DiagnosticService.Name; }
		}

		public bool MergingRight
		{
			get { return _mergingRight; }
		}

		public ApplicationComponentHost MergedOrderPreviewComponentHost
		{
			get { return _mergedOrderPreviewComponentHost; }
		}

		public void ToggleMergeDirection()
		{
			_mergingRight = !_mergingRight;

			var dryRunMergedOrder = SubmitMergeRequest(true);
			ShowMergedOrder(dryRunMergedOrder);
		}

		public void Accept()
		{
			try
			{
				SubmitMergeRequest(false);
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private OrderDetail SubmitMergeRequest(bool dryRun)
		{
			var sourceOrderRef = _mergingRight ? _order1Ref : _order2Ref;
			var destinationOrderRef = _mergingRight ? _order2Ref : _order1Ref;

			MergeOrderResponse response = null;
			Platform.GetService(
				delegate(IOrderEntryService service)
				{
					var request = new MergeOrderRequest(sourceOrderRef, destinationOrderRef) { DryRun = dryRun };
					response = service.MergeOrder(request);
				});

			if (!string.IsNullOrEmpty(response.DryRunFailureReason))
				this.Host.ShowMessageBox(response.DryRunFailureReason, MessageBoxActions.Ok);

			return response.DryRunMergedOrder;
		}

		private void ShowMergedOrder(OrderDetail mergedOrder)
		{
			if (mergedOrder == null)
			{
				_orderPreviewComponent.Context = null;
				_orderAdditionalInfoComponent.HealthcareContext = null;
				_orderAdditionalInfoComponent.OrderExtendedProperties = new Dictionary<string, string>();
				_attachmentSummaryComponent.OrderAttachments = new List<OrderAttachmentSummary>();
			}
			else
			{
				_orderPreviewComponent.Context = new OrderDetailViewComponent.OrderContext(mergedOrder);
				_orderAdditionalInfoComponent.HealthcareContext = mergedOrder;
				_orderAdditionalInfoComponent.OrderExtendedProperties = mergedOrder.ExtendedProperties;
				_attachmentSummaryComponent.OrderAttachments = mergedOrder.Attachments;
			}
		}
	}
}
