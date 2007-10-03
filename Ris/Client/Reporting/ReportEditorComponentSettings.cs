using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Client.Reporting
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ReportEditorComponentSettings
    {
        private ReportEditorComponentSettings()
        {
            ApplicationSettingsRegister.Instance.RegisterInstance(this);
        }
    }
}
