#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Memory and storage size units
    /// </summary>
    public enum SizeUnits
    {
        /// <summary>
        /// Bytes
        /// </summary>
        Bytes,

        /// <summary>
        /// Kilobytes
        /// </summary>
        Kilobytes,

        /// <summary>
        /// Megabytes
        /// </summary>
        Megabytes,

        /// <summary>
        /// Gigabytes
        /// </summary>
        Gigabytes
    }

    /// <summary>
    /// Provides convenience methods for querying system resources.
    /// </summary>
    public static class SystemResources
    {
        private const ulong OneKilobyte = 1024;
        private const ulong OneMegabyte = OneKilobyte*OneKilobyte;
        private const ulong OneGigabyte = OneKilobyte*OneMegabyte;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);


        public static long GetAvailableMemory(SizeUnits units)
        {
            var memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                ulong availableMemory = memStatus.ullAvailPhys;

                if (units == SizeUnits.Megabytes)
                    return (long) (availableMemory/OneMegabyte);

                if (units == SizeUnits.Kilobytes)
                    return (long) (availableMemory/OneKilobyte);

                if (units == SizeUnits.Bytes)
                    return (long) availableMemory;

                return (long) (availableMemory/OneGigabyte);
            }
            return 0;
        }

        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out long lpFreeBytesAvailableToCaller,
                                                      out long lpTotalNumberOfBytes, out long lpTotalNumberOfFreeBytes);

        public static DriveInformation GetDriveInformation(string path)
        {
            long available, total, free;
            bool result = GetDiskFreeSpaceEx(path, out available, out total, out free);

            if (result)
            {
                return new DriveInformation
                           {
                               RootDirectory = Path.GetPathRoot(path),
                               Total = total,
                               Free = free
                           };
            }

            throw new Win32Exception(string.Format("Unable to get drive information {0}. Error code: {1}", path, 0));
        }

        #region Nested type: MEMORYSTATUSEX

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint) Marshal.SizeOf(this);
            }
        }

        #endregion
    }

    public class DriveInformation
    {
        public string RootDirectory { get; set; }

        public long Total { get; set; }

        public long Free { get; set; }
    }
}