using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Application.Services
{

    [SettingsGroupDescription("Maintains information about a user's current working facility.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class WorkingFacilitySettings
    {
        ///<summary>
        /// Public constructor.  Server-side settings classes should be instantiated via constructor rather
        /// than using the <see cref="WorkingFacilitySettings.Default"/> property to avoid creating a static instance.
        ///</summary>
        public WorkingFacilitySettings()
        {
            // Note: server-side settings classes do not register in the <see cref="ApplicationSettingsRegistry"/>
        }
    }
}
