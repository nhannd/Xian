using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class MergedOrderDetailViewComponent : OrderDetailViewComponent
	{
		public MergedOrderDetailViewComponent()
		{
		}

		public MergedOrderDetailViewComponent(OrderDetail detail)
		{
			_context = detail;
		}

		protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.MergedOrderDetailPageUrl; }
		}
	}
}
