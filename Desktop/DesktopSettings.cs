using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop
{

    [SettingsGroupDescriptionAttribute("Settings that affect the appearance and behaviour of the Desktop")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class DesktopSettings
    {

        public DesktopSettings()
        {
            ApplicationSettingsRegister.Instance.RegisterInstance(this);
        }
    }
}
