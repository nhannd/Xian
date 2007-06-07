using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Healthcare.Alert
{
    [SettingsGroupDescription("Configures the Incomplete Patient Demographic Data alert")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class IncompleteDemographicDataAlertSettings
    {
        public IncompleteDemographicDataAlertSettings()
        {
        }
    }
}
