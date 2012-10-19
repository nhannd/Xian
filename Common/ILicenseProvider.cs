#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Represents a license information provider.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	public interface ILicenseProvider
	{
		/// <summary>
		/// Gets license information.
		/// </summary>
		void GetLicenseInfo(out string licenseKey, out string machineId);

		/// <summary>
		/// Sets the current license key.
		/// </summary>
		/// <param name="licenseKey"></param>
		void SetLicenseInfo(string licenseKey);

		/// <summary>
		/// Fired when the license information has changed.
		/// </summary>
		event EventHandler LicenseInfoChanged;
	}

	/// <summary>
	/// An extension point for <see cref="ILicenseProvider"/>s.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	[ExtensionPoint]
	public sealed class LicenseProviderExtensionPoint : ExtensionPoint<ILicenseProvider>
	{
		private LicenseProviderExtensionPoint() {}

		internal static ILicenseProvider CreateInstance()
		{
			try
			{
				// check for a license provider extension
				return (ILicenseProvider) new LicenseProviderExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return new LocalLicenseProvider();
			}
		}

		private sealed class LocalLicenseProvider : ILicenseProvider
		{
			public void GetLicenseInfo(out string licenseKey, out string machineId)
			{
				licenseKey = ApplicationSettingsExtensions.GetSharedPropertyValue(new LicenseSettings(), "LicenseKey").ToString();
				machineId = EnvironmentUtilities.MachineIdentifier;
			}

			public void SetLicenseInfo(string licenseKey)
			{
				var oldLicenseKey = ApplicationSettingsExtensions.GetSharedPropertyValue(new LicenseSettings(), "LicenseKey").ToString();
				ApplicationSettingsExtensions.SetSharedPropertyValue(new LicenseSettings(), "LicenseKey", licenseKey);

				if (oldLicenseKey != licenseKey)
					EventsHelper.Fire(LicenseInfoChanged, this, new EventArgs());
			}

			public event EventHandler LicenseInfoChanged;
		}
	}
}