using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
	public class OrderAdditionalInfoSummaryComponent : DHtmlComponent
	{
		private IDictionary<string, string> _orderExtendedProperties;

		public OrderAdditionalInfoSummaryComponent(IDictionary<string, string> orderExtendedProperties)
		{
			_orderExtendedProperties = orderExtendedProperties;
		}

		public OrderAdditionalInfoSummaryComponent()
		{
			_orderExtendedProperties = new Dictionary<string, string>();
		}

		/// <summary>
		/// Gets or sets the dictionary of order extended properties that this component will
		/// use to store data.
		/// </summary>
		public IDictionary<string, string> OrderExtendedProperties
		{
			get { return _orderExtendedProperties; }
			set
			{
				 _orderExtendedProperties = value;

				// refresh the page
				NotifyAllPropertiesChanged();
			}
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.OrderAdditionalInfoReadonlyPageUrl);
			base.Start();
		}

		protected override IDictionary<string, string> TagData
		{
			get
			{
				return _orderExtendedProperties;
			}
		}
	}
}