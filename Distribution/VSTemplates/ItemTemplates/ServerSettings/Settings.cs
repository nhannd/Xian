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

namespace $rootnamespace$ {
    
    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class $safeitemname$
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
		/// <remarks>
		/// Server-side settings classes should be instantiated via constructor rather
        /// than using the <see cref="$safeitemname$.Default"/> property to avoid creating a static instance.
		/// </remarks>
        public $safeitemname$()
        {
            // Note: server-side settings classes do not register in the ApplicationSettingsRegistry
        }
    }
}
