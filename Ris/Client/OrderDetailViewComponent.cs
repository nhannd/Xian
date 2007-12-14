using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public class OrderDetailViewComponent : DHtmlComponent
    {
        public OrderDetailViewComponent()
        {
        }

        public override void Start()
        {
            SetUrl(OrderDetailViewComponentSettings.Default.OrderDetailPageUrl);
            base.Start();
        }

        public void Refresh()
        {
            NotifyAllPropertiesChanged();
        }
    }
}
