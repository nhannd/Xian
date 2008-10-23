using ClearCanvas.Common;
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
		private ChildComponentHost _additionalInfoComponentHost;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;

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
			_orderNoteComponent.Notes = _context.OrderNotes;
			_orderNotesComponentHost = new ChildComponentHost(this.Host, _orderNoteComponent);
			_orderNotesComponentHost.StartComponent();

			_protocolSummaryComponentHost = new ChildComponentHost(this.Host, new ProtocolSummaryComponent(_context));
			_protocolSummaryComponentHost.StartComponent();

			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent();
			_orderAdditionalInfoComponent.OrderExtendedProperties = _context.OrderExtendedProperties;
			_orderAdditionalInfoComponent.HealthcareContext = _worklistItem;
			_additionalInfoComponentHost = new ChildComponentHost(this.Host, _orderAdditionalInfoComponent);
			_additionalInfoComponentHost.StartComponent();

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

            if (_additionalInfoComponentHost != null)
            {
                _additionalInfoComponentHost.StopComponent();
                _additionalInfoComponentHost = null;
            }

            base.Stop();
        }

		public ApplicationComponentHost AdditionalInfoHost
		{
			get { return _additionalInfoComponentHost; }
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
