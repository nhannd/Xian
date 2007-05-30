using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClearCanvas.Ris.Client
{
    [SettingsGroupDescriptionAttribute("Provides base URL for HtmlApplicationComponent")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class WebResourcesSettings
    {
        public WebResourcesSettings()
        {
        }
    }
}
