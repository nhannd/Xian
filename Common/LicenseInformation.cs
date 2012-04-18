#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Provides access to product licensing information.
	/// </summary>
	public static class LicenseInformation
	{
		private static readonly object _syncRoot = new object();

		private static ILicenseProvider _licenseProvider;
		private static ILicenseDetailsProvider _licenseDetailsProvider;

		private static string _licenseKey;
		private static string _machineIdentifier;
		private static int _sessionCount;

		private static void CheckLicenseProvider()
		{
			if (_licenseProvider != null) return;

			lock (_syncRoot)
			{
				if (_licenseProvider != null) return;
				_licenseProvider = LicenseProviderExtensionPoint.CreateInstance();
			}
		}

		private static void CheckLicenseDetailsProvider()
		{
			if (_licenseDetailsProvider != null) return;

			lock (_syncRoot)
			{
				if (_licenseDetailsProvider != null) return;
				_licenseDetailsProvider = LicenseDetailsProviderExtensionPoint.CreateInstance();
			}
		}

		/// <summary>
		/// Gets the number of the concurrent active sessions.
		/// </summary>
		public static int SessionCount
		{
			get
			{
				CheckLicenseProvider();

				lock (_syncRoot)
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
		/// Gets a unique identifier for the installation.
		/// </summary>
		public static string MachineIdentifier
		{
			get
			{
				CheckLicenseProvider();

				lock (_syncRoot)
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
		/// Gets or sets the product license key.
		/// </summary>
		public static string LicenseKey
		{
			get
			{
				CheckLicenseProvider();

				lock (_syncRoot)
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
				CheckLicenseProvider();

				lock (_syncRoot)
				{
					_licenseKey = value;
					_licenseProvider.SetLicenseInfo(_licenseKey);
				}
			}
		}

		/// <summary>
		/// Gets a string indicating the product license type.
		/// </summary>
		public static string LicenseType
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.LicenseType ?? string.Empty;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating the licensed diagnostic use of the product.
		/// </summary>
		public static LicenseDiagnosticUse DiagnosticUse
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.DiagnosticUse;
				}
			}
		}

		/// <summary>
		/// Gets the date when the product was first run.
		/// </summary>
		public static DateTime? FirstRun
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.FirstRun;
				}
			}
		}

		/// <summary>
		/// Gets the status if the license is for a limited-use trial.
		/// </summary>
		/// <param name="timeRemaining">Time remaining in trial period.</param>
		/// <returns>True if license is for a limited-use trial; False otherwise.</returns>
		public static bool GetTrialStatus(out TimeSpan? timeRemaining)
		{
			CheckLicenseDetailsProvider();

			lock (_syncRoot)
			{
				return _licenseDetailsProvider.GetTrialStatus(out timeRemaining);
			}
		}

		public static bool IsEvaluation
		{
			get
			{
				TimeSpan? ignore;
				return GetTrialStatus(out ignore);
			}
		}

		public static DateTime? ExpiryTime
		{
			get
			{
				CheckLicenseDetailsProvider();

				lock (_syncRoot)
				{
					return _licenseDetailsProvider.GetExpiryTime();
				}
			}
		}

		public static bool Expired
		{
			get
			{
				var expiryTime = ExpiryTime;
				return expiryTime.HasValue && expiryTime.Value < DateTime.Now;
			}
		}

		/// <summary>
		/// Forces license information to be reloaded when it is requested next time
		/// </summary>
		public static void Reset()
		{
			_machineIdentifier = null; // will force reload when requested
		}
	}
}