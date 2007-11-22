using System.Configuration;

namespace ClearCanvas.Ris.Client
{

    /// <summary>
    /// Provides URL for RIS application web services.  These settings are stored locally in the app.config file.
    /// </summary>
    [SettingsGroupDescriptionAttribute("Provides location of the RIS application web services.")]
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class WebServicesSettings
    {
        private WebServicesSettings()
        {
        }
    }
}
