using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Reporting
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ReportDocumentSettings
    {
        private ReportDocumentSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
