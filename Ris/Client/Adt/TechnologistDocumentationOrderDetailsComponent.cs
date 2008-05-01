using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client.Adt
{
	/// <summary>
	/// Extension point for views onto <see cref="TechnologistDocumentationOrderDetailsComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class TechnologistDocumentationOrderDetailsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// TechnologistDocumentationOrderDetailsComponent class
	/// </summary>
	[AssociateView(typeof(TechnologistDocumentationOrderDetailsComponentViewExtensionPoint))]
	public class TechnologistDocumentationOrderDetailsComponent : ApplicationComponent
	{
        #region ProtocolSummaryComponent class

        public class ProtocolSummaryComponent : DHtmlComponent
        {
            [DataContract]
            public class HealthcareContext : DataContractBase
            {
                internal HealthcareContext(EntityRef orderRef)
                {
                    this.OrderRef = orderRef;
                }

                [DataMember]
                public EntityRef OrderRef;

            }

            private readonly HealthcareContext _healthcareContext;

            public ProtocolSummaryComponent(ITechnologistDocumentationContext context)
            {
                _healthcareContext = new HealthcareContext(context.OrderRef);
            }

            public override void Start()
            {
                SetUrl(WebResourcesSettings.Default.ProtocolSummaryUrl);
                base.Start();
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return _healthcareContext;
            }
        }

        #endregion


	    private OrderNoteSummaryComponent _orderNoteComponent;
		private ChildComponentHost _orderNotesComponentHost;
		private ChildComponentHost _protocolSummaryComponentHost;
		private ChildComponentHost _additionalInfoComponentHost;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;

	    private readonly ITechnologistDocumentationContext _context;

		/// <summary>
		/// Constructor
		/// </summary>
		public TechnologistDocumentationOrderDetailsComponent(ITechnologistDocumentationContext context)
		{
		    _context = context;
		}

		public override void Start()
		{
            _orderNoteComponent = new OrderNoteSummaryComponent(OrderNoteCategory.General);
            _orderNoteComponent.Notes = _context.OrderNotes;
            _orderNotesComponentHost = new ChildComponentHost(this.Host, _orderNoteComponent);
			_orderNotesComponentHost.StartComponent();

			_protocolSummaryComponentHost = new ChildComponentHost(this.Host, new ProtocolSummaryComponent(_context));
			_protocolSummaryComponentHost.StartComponent();

			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent(_context.OrderExtendedProperties);
			_additionalInfoComponentHost = new ChildComponentHost(this.Host, _orderAdditionalInfoComponent);
			_additionalInfoComponentHost.StartComponent();

			base.Start();
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
