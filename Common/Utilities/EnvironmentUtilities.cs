#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// A unique identifier for the machine based on the processor ID and drive ID
	/// </summary>
	public sealed class EnvironmentUtilities
	{
		/// <summary>
		/// A unique identifier for the machine based on the processor ID and drive ID
		/// </summary>
		public static string MachineIdentifier
		{
			get
			{
				string cpuInfo = string.Empty;
				ManagementClass processor = new ManagementClass("win32_processor");
				ManagementObjectCollection moc = processor.GetInstances();

				foreach (ManagementObject mo in moc)
				{
					cpuInfo = mo.Properties["processorID"].Value.ToString();
					break;
				}

				const string drive = "C";
				ManagementObject disk = new ManagementObject(
					@"win32_logicaldisk.deviceid=""" + drive + @":""");
				disk.Get();
				string volumeSerial = disk["VolumeSerialNumber"].ToString();

				byte[] data = Encoding.Default.GetBytes(cpuInfo + "_" + volumeSerial);

				SHA256 sha = new SHA256Managed();
				byte[] result = sha.ComputeHash(data);

				return Convert.ToBase64String(result);
			}
		}

	}
}
