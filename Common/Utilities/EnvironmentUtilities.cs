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
using System.Globalization;
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
					var input = string.Format("CLEARCANVASRTW::{0}::{1}::{2}::{3}::{4}", GetProcessorId(), GetMotherboardSerial(), GetDiskSignature(), GetBiosSerial(), GetSystemUuid());
					using (var sha256 = new SHA256Managed())
					{
						_machineIdentifier = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
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
							if (processorId != null) processorId = processorId.Trim();
							if (!string.IsNullOrEmpty(processorId))
								return processorId;
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
							return string.Format(CultureInfo.InvariantCulture, "CPU-{0}-{1}-{2:X2}-{3:X2}-{4}-{5:X4}", manufacturer, addressWidth, architecture, family, level, revision);
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

		private static string GetBiosSerial()
		{
			try
			{
				// read the s/n of BIOS
				using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var bios in results)
						{
							var serialNumber = ReadString(bios, "SerialNumber");
							if (serialNumber != null) serialNumber = serialNumber.Trim();
							if (!string.IsNullOrEmpty(serialNumber))
								return serialNumber;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve BIOS serial number.");
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
							if (serialNumber != null) serialNumber = serialNumber.Trim();
							if (!string.IsNullOrEmpty(serialNumber))
								return serialNumber;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve baseboard serial number.");
			}
			return string.Empty;
		}

		private static string GetDiskSignature()
		{
			try
			{
				// identify the disk drives sorted by hardware order
				var diskDrives = new SortedList<uint, uint>();
				using (var searcher = new ManagementObjectSearcher("SELECT Index, Signature FROM Win32_DiskDrive"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var diskDrive in results)
						{
							var index = ReadUInt32(diskDrive, "Index");
							var signature = ReadUInt32(diskDrive, "Signature");
							if (index.HasValue && signature.HasValue)
								diskDrives.Add(index.Value, signature.Value);
						}
					}
				}

				// use the signature of the first physical disk drive
				foreach (var diskDriveId in diskDrives)
					return diskDriveId.Value.ToString("X8", CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve disk drive signature.");
			}
			return string.Empty;
		}

		private static string GetSystemUuid()
		{
			try
			{
				// read the UUID of the system
				using (var searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct"))
				{
					using (var results = new ManagementObjectSearcherResults(searcher))
					{
						foreach (var system in results)
						{
							var uuid = ReadString(system, "UUID");
							if (uuid != null) uuid = uuid.Trim();
							if (!string.IsNullOrEmpty(uuid))
								return uuid;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve system UUID.");
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