using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Client.Adt
{
    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class TechnologistDocumentationComponentSettings
    {
        private TechnologistDocumentationComponentSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
