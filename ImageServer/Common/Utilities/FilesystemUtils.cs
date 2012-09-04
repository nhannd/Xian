#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;


namespace ClearCanvas.ImageServer.Common.Utilities
{
    /// <summary>
    /// Provides methods to determine existence of a folder and its properties
    /// </summary>
    public class FilesystemUtils
    {
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        static extern int GetDiskFreeSpaceEx(
                                 string lpDirectoryName,
                                 out ulong lpFreeBytesAvailable,
                                 out ulong lpTotalNumberOfBytes,
                                 out ulong lpTotalNumberOfFreeBytes);


        /// <summary>
        /// Checks if a specified directory exists on the network and accessible from local machine.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool DirectoryExists(String dir, int timeout)
        {
            bool exists = false;


            if (timeout > 0)
            {
                var t = new Thread(delegate()
                                       {
                                           exists = Directory.Exists(dir);
                                       });

                t.Start();
                t.Join(timeout);
                t.Abort();
            }
            else
            {
                exists = Directory.Exists(dir);
            }

            return exists;
        }

        
        /// <summary>
        /// Gets information of a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FilesystemInfo GetDirectoryInfo(String path)
        {
            var fsInfo = new FilesystemInfo
                             {
                                 Path = path,
                                 Exists = DirectoryExists(path, 1000)
                             };

            if (fsInfo.Exists)
            {
                ulong available;
                ulong total;
                ulong free;
                GetDiskFreeSpaceEx(path, out available, out total, out free);

                fsInfo.FreeSizeInKB = available / 1024;
                fsInfo.SizeInKB = total / 1024;

            }

            return fsInfo;
        }
    }
}
