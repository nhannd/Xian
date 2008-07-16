using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ReportingOrderDetailViewComponent : OrderDetailViewComponent
	{
		public ReportingOrderDetailViewComponent()
		{
		}

		public ReportingOrderDetailViewComponent(EntityRef orderRef)
			: base(orderRef)
		{
		}
		
		protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.ReportingOrderDetailPageUrl; }
		}
	}
}
