using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public class BiographyOrderDetailViewComponent : OrderDetailViewComponent
	{
		public BiographyOrderDetailViewComponent()
		{
		}

		public BiographyOrderDetailViewComponent(EntityRef orderRef)
			: base(orderRef)
		{
		}
		
		protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.BiographyOrderDetailPageUrl; }
		}
	}
}
