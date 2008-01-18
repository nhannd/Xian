using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.SpeechMagic
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ReportEditorComponentSettings
    {
        private ReportEditorComponentSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }

        ~ReportEditorComponentSettings()
        {
            ApplicationSettingsRegistry.Instance.UnregisterInstance(this);
        }
    }
}
