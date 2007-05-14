using System;
using System.Configuration;

namespace ClearCanvas.Ris.Client
{

    [SettingsGroupDescriptionAttribute("Allows configuration of display format for common healthcare objects")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class FormatSettings
    {

        public FormatSettings()
        {
        }
    }
}
