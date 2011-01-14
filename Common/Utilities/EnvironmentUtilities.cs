#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Generic environment utilities.
	/// </summary>
	public sealed class EnvironmentUtilities
	{
		private static string _machineIdentifier;

		/// <summary>
		/// Gets a unique identifier for the machine.
		/// </summary>
		public static string MachineIdentifier
		{
			get
			{
				if (_machineIdentifier == null)
				{
					var input = Encoding.Default.GetBytes(string.Format("CLEARCANVASRTW_{0}_{1}_{2}", GetProcessorId(), GetMotherboardSerial(), GetDiskSerial()));
					using (var sha256 = new SHA256Managed())
					{
						_machineIdentifier = Convert.ToBase64String(sha256.ComputeHash(input));
					}
				}
				return _machineIdentifier;
			}
		}

		private static string GetProcessorId()
		{
			try
			{
				// read the cpuid of the first processor
				using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var processor in results)
						{
							var processorId = processor["ProcessorId"];
							if (processorId != null)
								return processorId.ToString().Trim();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve processor ID.");
			}
			return string.Empty;
		}

		private static string GetMotherboardSerial()
		{
			try
			{
				// read the s/n of the motherboard
				using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var motherboard in results)
						{
							var serialNumber = motherboard["SerialNumber"];
							if (serialNumber != null)
								return serialNumber.ToString().Trim();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve baseboard serial.");
			}
			return string.Empty;
		}

		private static string GetDiskSerial()
		{
			try
			{
				// identify the disk drives sorted by hardware order
				var diskDriveIds = new SortedList<uint, string>();
				using (var searcher = new ManagementObjectSearcher("SELECT Index, DeviceID FROM Win32_DiskDrive"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var diskDrive in results)
						{
							var index = diskDrive["Index"];
							var deviceId = diskDrive["DeviceId"];
							if (index == null || deviceId == null)
								continue;

							diskDriveIds.Add((uint) index, deviceId.ToString());
						}
					}
				}

				// read the s/n of the first disk drive
				foreach (var diskDriveId in diskDriveIds.Values)
				{
					var query = string.Format(
						"ASSOCIATORS OF {0}Win32_DiskDrive.DeviceID=\"{2}\"{1} WHERE ResultClass = Win32_PhysicalMedia", '{', '}',
						WqlEscape(diskDriveId));
					using (var searcher = new ManagementObjectSearcher(query))
					{
						using (var results = new ManagementObjectSearcherResults(searcher))
						{
							foreach (var media in results)
							{
								var serialNumber = media["SerialNumber"];
								if (serialNumber != null)
									return serialNumber.ToString().Trim();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve disk drive serial.");
			}
			return string.Empty;
		}

		private static string WqlEscape(string literal)
		{
			// there's probably more characters that need escaping for WQL, but we only use this for deviceID
			// and it doesn't appear that there are any other likely character candidates for escaping
			return literal.Replace("\\", "\\\\").Replace("\"", "\"\"");
		}

		/// <summary>
		/// Utility class for proper disposal of results from WMI queries.
		/// </summary>
		private class ManagementObjectSearcherResults : IEnumerable<ManagementBaseObject>, IDisposable
		{
			private List<ManagementBaseObject> _results;

			public ManagementObjectSearcherResults(ManagementObjectSearcher searcher)
			{
				// the result collection is a wrapper around a COM enumerator that constructs new COM objects per iteration
				// because of this, don't ever enumerate the collection directly - cache the results from one iteration and enumerate the cache instead
				_results = new List<ManagementBaseObject>();
				using (var results = searcher.Get())
				{
					foreach (var result in results)
						_results.Add(result);
				}
			}

			public void Dispose()
			{
				if (_results != null)
				{
					foreach (var result in _results)
						result.Dispose();
					_results.Clear();
					_results = null;
				}
			}

			public IEnumerator<ManagementBaseObject> GetEnumerator()
			{
				return _results.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}