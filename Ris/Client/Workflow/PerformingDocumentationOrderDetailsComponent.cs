#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="PerformingDocumentationOrderDetailsComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PerformingDocumentationOrderDetailsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PerformingDocumentationOrderDetailsComponent class
	/// </summary>
	[AssociateView(typeof(PerformingDocumentationOrderDetailsComponentViewExtensionPoint))]
	public class PerformingDocumentationOrderDetailsComponent : ApplicationComponent
	{
		#region ProtocolSummaryComponent class

		public class ProtocolSummaryComponent : OrderDetailViewComponent
		{
			public ProtocolSummaryComponent(IPerformingDocumentationContext context)
				: base(context.OrderRef)
			{
			}

			protected override string PageUrl
			{
				get { return WebResourcesSettings.Default.PerformingOrderDetailPageUrl; }
			}
		}

		#endregion

		private OrderNoteSummaryComponent _orderNoteComponent;
		private ChildComponentHost _orderNotesComponentHost;
		private ChildComponentHost _protocolSummaryComponentHost;

		private ChildComponentHost _rightHandComponentContainerHost;
		private TabComponentContainer _rightHandComponentContainer;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;
		private AttachedDocumentPreviewComponent _orderAttachmentsComponent;

		private readonly IPerformingDocumentationContext _context;
		private readonly DataContractBase _worklistItem;

		/// <summary>
		/// Constructor
		/// </summary>
		public PerformingDocumentationOrderDetailsComponent(IPerformingDocumentationContext context, DataContractBase worklistItem)
		{
			_context = context;
			_worklistItem = worklistItem;
		}

		public override void Start()
		{
			_orderNoteComponent = new OrderNoteSummaryComponent(OrderNoteCategory.General);
			_orderNotesComponentHost = new ChildComponentHost(this.Host, _orderNoteComponent);
			_orderNotesComponentHost.StartComponent();
			_orderNoteComponent.Notes = _context.OrderNotes;

			_protocolSummaryComponentHost = new ChildComponentHost(this.Host, new ProtocolSummaryComponent(_context));
			_protocolSummaryComponentHost.StartComponent();

			_rightHandComponentContainer = new TabComponentContainer();
			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent();
			_orderAdditionalInfoComponent.OrderExtendedProperties = _context.OrderExtendedProperties;
			_orderAdditionalInfoComponent.HealthcareContext = _worklistItem;
			_rightHandComponentContainer.Pages.Add(new TabPage(SR.TitleAdditionalInfo, _orderAdditionalInfoComponent));

			_orderAttachmentsComponent = new AttachedDocumentPreviewComponent(true, AttachedDocumentPreviewComponent.AttachmentMode.Order);
			_orderAttachmentsComponent.OrderRef = _context.OrderRef;
			_rightHandComponentContainer.Pages.Add(new TabPage(SR.TitleOrderAttachments, _orderAttachmentsComponent));

			_rightHandComponentContainerHost = new ChildComponentHost(this.Host, _rightHandComponentContainer);
			_rightHandComponentContainerHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_orderNotesComponentHost != null)
			{
				_orderNotesComponentHost.StopComponent();
				_orderNotesComponentHost = null;
			}

			if (_protocolSummaryComponentHost != null)
			{
				_protocolSummaryComponentHost.StopComponent();
				_protocolSummaryComponentHost = null;
			}

			if (_rightHandComponentContainerHost != null)
			{
				_rightHandComponentContainerHost.StopComponent();
				_rightHandComponentContainerHost = null;
			}

			base.Stop();
		}

		public ApplicationComponentHost RightHandComponentContainerHost
		{
			get { return _rightHandComponentContainerHost; }
		}

		public ApplicationComponentHost ProtocolHost
		{
			get { return _protocolSummaryComponentHost; }
		}

		public ApplicationComponentHost NotesHost
		{
			get { return _orderNotesComponentHost; }
		}

		internal void SaveData()
		{
			_orderAdditionalInfoComponent.SaveData();
		}
	}
}
