using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Healthcare.Alert
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class IncompleteDemographicDataAlertSettings
    {
        public IncompleteDemographicDataAlertSettings()
        {
            ApplicationSettingsRegister.Instance.RegisterInstance(this);
        }

        ~IncompleteDemographicDataAlertSettings()
        {
            ApplicationSettingsRegister.Instance.UnregisterInstance(this);
        }
    }
}
