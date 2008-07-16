using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public class BiographyVisitDetailViewComponent : VisitDetailViewComponent
	{
		public BiographyVisitDetailViewComponent()
		{
		}

		public BiographyVisitDetailViewComponent(EntityRef visitRef)
			: base(visitRef)
		{
		}
		
		protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.BiographyVisitDetailPageUrl; }
		}
	}
}
