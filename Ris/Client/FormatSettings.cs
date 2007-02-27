using System;
using System.Configuration;

namespace ClearCanvas.Ris.Client
{

    [SettingsGroupDescriptionAttribute("Settings that control the client display format of data entry fields")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class FormatSettings
    {

        public FormatSettings()
        {
        }
    }
}
