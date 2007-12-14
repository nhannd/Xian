using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public class BannerComponent : DHtmlComponent
    {
        public BannerComponent()
        {
        }

        public override void Start()
        {
            SetUrl(BannerComponentSettings.Default.BannerPageUrl);
            base.Start();
        }

        public void Refresh()
        {
            NotifyAllPropertiesChanged();
        }
    }
}
