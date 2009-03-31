using System.Configuration;

namespace ClearCanvas.Enterprise.Common
{
	// these settings must be stored in a config file, not in the configuration store
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class RemoteCoreServiceSettings
	{
        
    }
}
