using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ProtocollingOrderDetailViewComponent : OrderDetailViewComponent
	{
		public ProtocollingOrderDetailViewComponent()
		{
		}

		public ProtocollingOrderDetailViewComponent(EntityRef orderRef)
			: base(orderRef)
		{
		}

		protected override string PageUrl
		{
			get
			{
				return WebResourcesSettings.Default.ProtocollingOrderDetailPageUrl;
			}
		}
	}
}
