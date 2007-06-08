using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop
{

    [SettingsGroupDescriptionAttribute("Configures global exception handling behaviour")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ExceptionHandlerSettings
    {

        private ExceptionHandlerSettings()
        {
            ApplicationSettingsRegister.Instance.RegisterInstance(this);
        }
    }
}
