#region License

// Copyright (c) 2011, ClearCanvas Inc.
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

using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
    /// <summary>
    /// An interface for a license provider.
    /// </summary>
    /// <remarks>
    /// The framework provides an <see cref="ILicenseProvider"/>,
    /// which uses the <see cref="LicenseProviderExtensionPoint"/> internally.
    /// </remarks>
    public interface ILicenseProvider
    {
        /// <summary>
        /// Gets the current license and machine Id.
        /// </summary>
        void GetLicenseInfo(out string licenseKey, out string machineID, out int sessionCount);

        /// <summary>
        /// Sets the current license
        /// </summary>
        /// <param name="licenseKey"></param>
        void SetLicenseInfo(string licenseKey);
    }

    /// <summary>
    /// An extension point for <see cref="ILicenseProvider"/>s.
    /// </summary>
    /// <remarks>
    /// Used internally by the framework to create a <see cref="ILicenseProvider"/> for
    /// use by the application.
    /// </remarks>
    [ExtensionPoint]
    public sealed class LicenseProviderExtensionPoint : ExtensionPoint<ILicenseProvider>
    {
    }

    /// <summary>
    /// Internal class that uses the local settings as the license provider.
    /// </summary>
    internal class LocalLicenseProvider: ILicenseProvider
    {
        public void GetLicenseInfo(out string licenseKey, out string machineID, out int sessionCount)
        {
            licenseKey = ApplicationSettingsExtensions.GetSharedPropertyValue(new LicenseSettings(), "LicenseKey").ToString();
            machineID = EnvironmentUtilities.MachineIdentifier;
            sessionCount = 1;
        }

        public void SetLicenseInfo(string licenseKey)
        {
            ApplicationSettingsExtensions.SetSharedPropertyValue(new LicenseSettings(), "LicenseKey", licenseKey);
        }
    }

	/// <summary>
	/// Provides access to the current license key.
	/// </summary>
	public static class LicenseInformation
	{
	    private static readonly object SyncRoot = new object();
	    private static ILicenseProvider _licenseProvider;
        private static string _licenseKey;
	    private static string _machineIdentifier;
	    private static int _sessionCount;

        private static void CheckProvider()
        {
            if (_licenseProvider != null) return;

            lock (SyncRoot)
            {
                if (_licenseProvider != null) return;

                try
                {
                    // check for a license provider extension
                    LicenseProviderExtensionPoint xp = new LicenseProviderExtensionPoint();
                    _licenseProvider = (ILicenseProvider) xp.CreateExtension();
                }
                catch (NotSupportedException)
                {
                    // can't find time provider, default to local time
                    Platform.Log(LogLevel.Debug, SR.LogLicenseProviderNotFound);

                    _licenseProvider = new LocalLicenseProvider();
                }
            }
        }

        /// <summary>
        /// A count of the current active sessions
        /// </summary>
        public static int SessionCount
        {
            get
            {
                CheckProvider();

                lock (SyncRoot)
                {
                    if (_machineIdentifier == null)
                    {
                        _licenseProvider.GetLicenseInfo(out _licenseKey, out _machineIdentifier, out _sessionCount);
                    }
                    return _sessionCount;
                }
            }
        }

	    /// <summary>
        /// A unique identifier for the machine based on the processor ID and drive ID
        /// </summary>
	    public static string MachineIdentifier
	    {
	        get
	        {               
                CheckProvider();

                lock (SyncRoot)
                {
                    if (_machineIdentifier == null)
                    {
                        _licenseProvider.GetLicenseInfo(out _licenseKey, out _machineIdentifier, out _sessionCount);
                    }
                    return _machineIdentifier;
                }
	        }
	    }

		/// <summary>
		/// Gets and sets the license key string from the local app.config file.
		/// </summary>
		/// <remarks>
		/// Use this instead of the LicenseKey property in <see cref="LicenseSettings"/>
		/// </remarks>
		/// <returns></returns>
		public static string LicenseKey
		{
			get
			{
                CheckProvider();

                lock (SyncRoot)
                {
                    if (_machineIdentifier == null)
                    {
                        _licenseProvider.GetLicenseInfo(out _licenseKey, out _machineIdentifier, out _sessionCount);
                    }
                    return _licenseKey;
                }
			}
			set
			{
                CheckProvider();
                                
                lock (SyncRoot)
                {
                    _licenseKey = value;
                    _licenseProvider.SetLicenseInfo(_licenseKey);
                }
			}
		}
	}

	[SettingsGroupDescription("Settings that store the license information.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class LicenseSettings
	{
	}
}
