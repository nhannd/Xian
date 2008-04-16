using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using System.Collections.Generic;

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
		public class ProtocolSummaryComponent : DHtmlComponent
		{
			private readonly WorklistItemSummaryBase _worklistItem;

			public ProtocolSummaryComponent(WorklistItemSummaryBase worklistItem)
			{
				_worklistItem = worklistItem;
			}

			public override void Start()
			{
				SetUrl(WebResourcesSettings.Default.ProtocolSummaryUrl);
				base.Start();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _worklistItem;
			}
		}

		private ChildComponentHost _orderNotesComponentHost;
		private ChildComponentHost _protocolSummaryComponentHost;
		private ChildComponentHost _additionalInfoComponentHost;
		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;

		private readonly WorklistItemSummaryBase _worklistItem;
		private readonly IDictionary<string, string> _orderExtendedProperties;

		/// <summary>
		/// Constructor
		/// </summary>
		public TechnologistDocumentationOrderDetailsComponent(WorklistItemSummaryBase worklistItem, IDictionary<string, string> orderExtendedProperties)
		{
			_worklistItem = worklistItem;
			_orderExtendedProperties = orderExtendedProperties;
		}

		public override void Start()
		{
			_orderNotesComponentHost = new ChildComponentHost(this.Host, new OrderNoteSummaryComponent());
			_orderNotesComponentHost.StartComponent();

			_protocolSummaryComponentHost = new ChildComponentHost(this.Host, new ProtocolSummaryComponent(_worklistItem));
			_protocolSummaryComponentHost.StartComponent();

			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent(_orderExtendedProperties);
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
