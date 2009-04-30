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
                Thread t = new Thread(delegate()
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
            FilesystemInfo fsInfo = new FilesystemInfo();
            fsInfo.Path = path;
            fsInfo.Exists = DirectoryExists(path, 1000);

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
