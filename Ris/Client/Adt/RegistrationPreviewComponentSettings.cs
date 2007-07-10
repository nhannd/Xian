using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Client.Adt
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class RegistrationPreviewComponentSettings
    {
        private RegistrationPreviewComponentSettings()
        {
            ApplicationSettingsRegister.Instance.RegisterInstance(this);
        }

        ~RegistrationPreviewComponentSettings()
        {
            ApplicationSettingsRegister.Instance.UnregisterInstance(this);
        }
    }
}
