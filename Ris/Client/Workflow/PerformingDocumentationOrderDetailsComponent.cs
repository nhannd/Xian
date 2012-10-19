#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines an interface for providing custom pages to be displayed in the performing documentation order details section.
	/// </summary>
	public interface IPerformingDocumentationOrderDetailsPageProvider : IExtensionPageProvider<IPerformingDocumentationOrderDetailsPage, IPerformingDocumentationOrderDetailsContext>
	{
	}

	/// <summary>
	/// Defines an interface to a custom performing order details page.
	/// </summary>
	public interface IPerformingDocumentationOrderDetailsPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an interface for providing a custom page with access to the performing documentation context.
	/// </summary>
	public interface IPerformingDocumentationOrderDetailsContext
	{
		WorklistItemSummaryBase WorklistItem { get; }
		IDictionary<string, string> OrderExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the performing documentation order details section.
	/// </summary>
	[ExtensionPoint]
	public class PerformingDocumentationOrderDetailsPageProviderExtensionPoint : ExtensionPoint<IPerformingDocumentationOrderDetailsPageProvider>
	{
	}

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

		class PerformingDocumentationOrderDetailsContext : IPerformingDocumentationOrderDetailsContext
		{
			private readonly PerformingDocumentationOrderDetailsComponent _owner;

			public PerformingDocumentationOrderDetailsContext(PerformingDocumentationOrderDetailsComponent owner)
			{
				_owner = owner;
			}

			public WorklistItemSummaryBase WorklistItem
			{
				get { return _owner._worklistItem; }
			}

			public IDictionary<string, string> OrderExtendedProperties
			{
				get { return _owner._context.OrderExtendedProperties; }
			}
		}

		private OrderNoteSummaryComponent _orderNoteComponent;
		private ChildComponentHost _orderNotesComponentHost;
		private ChildComponentHost _protocolSummaryComponentHost;

		private ChildComponentHost _rightHandComponentContainerHost;
		private TabComponentContainer _rightHandComponentContainer;
		private AttachedDocumentPreviewComponent _orderAttachmentsComponent;

		private readonly IPerformingDocumentationContext _context;
		private readonly WorklistItemSummaryBase _worklistItem;

		private readonly List<IPerformingDocumentationOrderDetailsPage> _extensionPages = new List<IPerformingDocumentationOrderDetailsPage>();

		/// <summary>
		/// Constructor
		/// </summary>
		public PerformingDocumentationOrderDetailsComponent(IPerformingDocumentationContext context, WorklistItemSummaryBase worklistItem)
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
			_orderAttachmentsComponent = new AttachedDocumentPreviewComponent(true, AttachmentSite.Order) {OrderRef = _context.OrderRef};
			_rightHandComponentContainer.Pages.Add(new TabPage(SR.TitleOrderAttachments, _orderAttachmentsComponent));

			// instantiate all extension pages
			foreach (IPerformingDocumentationOrderDetailsPageProvider pageProvider in new PerformingDocumentationOrderDetailsPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new PerformingDocumentationOrderDetailsContext(this)));
			}

			// add extension pages to container and set initial context
			// the container will start those components if the user goes to that page
			foreach (var page in _extensionPages)
			{
				_rightHandComponentContainer.Pages.Add(new TabPage(page.Path, page.GetComponent()));
			}

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
			foreach (var extensionPage in _extensionPages)
			{
				extensionPage.Save();
			}
		}
	}
}
