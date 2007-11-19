using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Remembers settings for the login dialog.  These settings are stored on the local machine. Although they 
    /// are "user"-scoped settings, they are not actually associated with a RIS user, rather they are associated
    /// with the Windows login of the local machine.
    /// </summary>
    [SettingsGroupDescriptionAttribute("Stores settings for the login dialog on the local machine.")]
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class LoginDialogSettings
    {
        private LoginDialogSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
