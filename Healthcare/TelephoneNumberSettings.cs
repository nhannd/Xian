using System;
using System.Configuration;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Settings that affect the behaviour of the <see cref="TelephoneNumber"/> class.
    /// </summary>
    [SettingsGroupDescriptionAttribute("Control the behaviour of telephone numbers")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class TelephoneNumberSettings
    {

        public TelephoneNumberSettings()
        {
        }
    }
}
