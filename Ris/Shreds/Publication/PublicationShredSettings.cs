using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Shreds.Publication
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class PublicationShredSettings
    {
        ///<summary>
        /// Public constructor.  Server-side settings classes should be instantiated via constructor rather
        /// than using the <see cref="PublicationShredSettings.Default"/> property to avoid creating a static instance.
        ///</summary>
        public PublicationShredSettings()
        {
            // Note: server-side settings classes do not register in the <see cref="ApplicationSettingsRegistry"/>
        }
    }
}
