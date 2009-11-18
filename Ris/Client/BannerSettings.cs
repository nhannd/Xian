#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Provides application settings for banner.
    /// </summary>
    /// <remarks>
    /// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
    /// settings need to be manually added to this class.
    /// </remarks>
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    public sealed class BannerSettings : global::System.Configuration.ApplicationSettingsBase
    {
        private static BannerSettings defaultInstance = ((BannerSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new BannerSettings())));

        private BannerSettings()
        {
            ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }

        public static BannerSettings Default {
            get {
                return defaultInstance;
            }
        }

        /// <summary>
        /// Defined the height of banner in pixels.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Defined the height of banner in pixels.")]
        [global::System.Configuration.DefaultSettingValueAttribute("95")]
        public int BannerHeight {
            get {
                return ((int)(this["BannerHeight"]));
            }
        }
    }
}
