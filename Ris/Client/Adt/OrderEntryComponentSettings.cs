using System;
using System.Configuration;

namespace ClearCanvas.Ris.Client.Adt
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescriptionAttribute("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class OrderEntryComponentSettings
    {

        public OrderEntryComponentSettings()
        {
        }
    }
}
