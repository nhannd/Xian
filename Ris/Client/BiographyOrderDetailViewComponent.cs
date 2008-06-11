using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
	public class BiographyOrderDetailViewComponent : OrderDetailViewComponent
	{
		protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.BiographyOrderDetailPageUrl; }
		}
	}
}
