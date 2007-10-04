using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Client
{

    [SettingsGroupDescriptionAttribute("Allows configuration of display format for common healthcare objects")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class FormatSettings
    {

        private FormatSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
