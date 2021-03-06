#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Ris.Shreds.Publication
{

    [SettingsGroupDescription("Settings that configure the behaviour of the Report Publication Shred.")]
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
