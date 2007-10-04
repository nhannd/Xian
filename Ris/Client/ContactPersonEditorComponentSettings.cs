using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Client
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescriptionAttribute("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ContactPersonEditorComponentSettings
    {

        private ContactPersonEditorComponentSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
