using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public class OrderSummaryViewComponent : DHtmlComponent
    {
        public OrderSummaryViewComponent()
        {
        }

        public override void Start()
        {
            SetUrl(OrderSummaryViewComponentSettings.Default.OrderSummaryPageUrl);
            base.Start();
        }

        public void Refresh()
        {
            NotifyAllPropertiesChanged();
        }
    }
}
