using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{
    [SettingsGroupDescription("Configures behaviour of the Reporting component.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ReportingSettings
    {
		private ReportingSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
