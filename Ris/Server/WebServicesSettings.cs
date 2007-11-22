using System.Configuration;

namespace ClearCanvas.Ris.Server
{
    /// <summary>
    /// Parameters for the web services exposed by this server. Note that these settings
    /// are stored in the app.config file, not in the enterprise config store.
    /// </summary>
    [SettingsGroupDescription("Parameters for the web services exposed by this server.")]
    internal sealed partial class WebServicesSettings
    {
        
        public WebServicesSettings()
        {
        }
        
    }
}
