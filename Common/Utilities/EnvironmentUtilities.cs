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
					var input = string.Format("CLEARCANVASRTW::{0}::{1}::{2}", GetProcessorId(), GetMotherboardSerial(), GetDiskSerial());
					using (var sha256 = new SHA256Managed())
					{
						_machineIdentifier = Convert.ToBase64String(sha256.ComputeHash(Encoding.Default.GetBytes(input)));
					}
				}
				return _machineIdentifier;
			}
		}

		private static string GetProcessorId()
		{
			try
			{
				// read the CPUID of the first processor
				using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var processor in results)
						{
							var processorId = ReadString(processor, "ProcessorId");
							if (!string.IsNullOrEmpty(processorId))
								return processorId.Trim();
						}
					}
				}

				// if the processor doesn't support the CPUID opcode, concatenate some immutable characteristics of the processor
				using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, AddressWidth, Architecture, Family, Level, Revision FROM Win32_Processor"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var processor in results)
						{
							var manufacturer = ReadString(processor, "Manufacturer");
							var addressWidth = ReadUInt16(processor, "AddressWidth");
							var architecture = ReadUInt16(processor, "Architecture");
							var family = ReadUInt16(processor, "Family");
							var level = ReadUInt16(processor, "Level");
							var revision = ReadUInt16(processor, "Revision");
							return string.Format("CPU-{0}-{1}-{2:X2}-{3:X2}-{4}-{5:X4}", manufacturer, addressWidth, architecture, family, level, revision);
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
							var serialNumber = ReadString(motherboard, "SerialNumber");
							if (!string.IsNullOrEmpty(serialNumber))
								return serialNumber.Trim();
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
							var index = ReadUInt32(diskDrive, "Index");
							var deviceId = ReadString(diskDrive, "DeviceId");
							if (index.HasValue && !string.IsNullOrEmpty(deviceId))
								diskDriveIds.Add(index.Value, deviceId);
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
								var serialNumber = ReadString(media, "SerialNumber");
								if (!string.IsNullOrEmpty(serialNumber))
									return serialNumber.Trim();
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

		private static string ReadString(ManagementBaseObject wmiObject, string propertyName)
		{
			try
			{
				var value = wmiObject[propertyName];
				if (value != null)
					return value.ToString();
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, @"WMI property ""{0}"" was not in the expected format.", propertyName);
			}
			return null;
		}

		private static ushort? ReadUInt16(ManagementBaseObject wmiObject, string propertyName)
		{
			try
			{
				var value = wmiObject[propertyName];
				if (value != null)
					return (ushort) value;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, @"WMI property ""{0}"" was not in the expected format.", propertyName);
			}
			return null;
		}

		private static uint? ReadUInt32(ManagementBaseObject wmiObject, string propertyName)
		{
			try
			{
				var value = wmiObject[propertyName];
				if (value != null)
					return (uint) value;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, @"WMI property ""{0}"" was not in the expected format.", propertyName);
			}
			return null;
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