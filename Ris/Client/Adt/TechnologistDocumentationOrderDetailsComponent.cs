using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

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
    public class TechnologistDocumentationOrderDetailsComponent : ApplicationComponent, IDocumentationPage
    {
        public class ProtocolSummaryComponent : DHtmlComponent
        {
            private readonly WorklistItemSummaryBase _worklistItem;

            public ProtocolSummaryComponent(WorklistItemSummaryBase worklistItem)
            {
                _worklistItem = worklistItem;
            }

            public override void Start()
            {
                SetUrl(TechnologistDocumentationComponentSettings.Default.ProtocolSummaryUrl);
                base.Start();
            }

            public void Refresh()
            {
                NotifyAllPropertiesChanged();
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return _worklistItem;
            }
        }

        private ChildComponentHost _orderNotesComponentHost;
        private ChildComponentHost _protocolSummaryComponentHost;
        
        private readonly WorklistItemSummaryBase _worklistItem;

        /// <summary>
        /// Constructor
        /// </summary>
        public TechnologistDocumentationOrderDetailsComponent(WorklistItemSummaryBase worklistItem)
        {
            _worklistItem = worklistItem;
        }

        public override void Start()
        {
            _orderNotesComponentHost = new ChildComponentHost(this.Host, new OrderNoteSummaryComponent());
            _orderNotesComponentHost.StartComponent();

            _protocolSummaryComponentHost = new ChildComponentHost(this.Host, new ProtocolSummaryComponent(_worklistItem));
            _protocolSummaryComponentHost.StartComponent();

            // TODO prepare the component for its live phase
            base.Start();
        }

        public ApplicationComponentHost ProtocolHost
        {
            get { return _protocolSummaryComponentHost; }
        }

        public ApplicationComponentHost NotesHost
        {
            get { return _orderNotesComponentHost; }
        }

        #region IDocumentationPage Members

        public string Title
        {
            get { return "Order Details"; }
        }

        public IApplicationComponent Component
        {
            get { return this; }
        }

        #endregion
    }
}
