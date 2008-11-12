using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Application.Services
{

    [SettingsGroupDescription("Configures behaviour of order notes and note-boxes.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class OrderNoteSettings
    {
        ///<summary>
        /// Public constructor.  Server-side settings classes should be instantiated via constructor rather
        /// than using the <see cref="OrderNoteSettings.Default"/> property to avoid creating a static instance.
        ///</summary>
        public OrderNoteSettings()
        {
            // Note: server-side settings classes do not register in the <see cref="ApplicationSettingsRegistry"/>
        }
    }
}
