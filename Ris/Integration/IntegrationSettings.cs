using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Integration
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class IntegrationSettings
    {
        private IntegrationSettings()
        {
            ApplicationSettingsRegister.Instance.RegisterInstance(this);
        }

        ~IntegrationSettings()
        {
            ApplicationSettingsRegister.Instance.UnregisterInstance(this);
        }
    }
}
