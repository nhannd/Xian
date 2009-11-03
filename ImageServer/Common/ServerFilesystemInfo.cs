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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common
{
    public class ServerFilesystemInfo
    {
        #region Private Members
        private Filesystem _filesystem;
        private readonly object _lock = new object();
        private float _freeBytes;
        private float _totalBytes;
        private bool _online;
        private DateTime _lastOfflineLog = Platform.Time.AddDays(-1);
        #endregion

        #region Public Properties
        /// <summary>
        /// The <see cref="Filesystem"/> domain object.
        /// </summary>
        public Filesystem Filesystem
        {
            get { return _filesystem; }
            internal set { _filesystem = value; }
        }

        /// <summary>
        /// Is the filesystem Online?
        /// </summary>
        public bool Online
        {
            get { return _online; }
        }

        /// <summary>
        /// Returns a boolean value indicating whether the filesystem is writable.
        /// </summary>
        public bool Writeable
        {
            get
            {
                if (!_online || _filesystem.ReadOnly || !_filesystem.Enabled)
                    return false;

                return !Full;
            }
        }

        public bool ReadOnly
        {
            get { return _filesystem.ReadOnly; }
        }
        
        /// <summary>
        /// Returns a boolean value indicating whether the filesystem is readonly.
        /// </summary>
        public bool Readable
        {
            get
            {
                if (!_online || _filesystem.WriteOnly || !_filesystem.Enabled)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Returns a boolean value indicating whether the filesystem is full.
        /// </summary>
        public bool Full
        {
            get
            {
                return FreeBytes / 1024f /1024f < Settings.Default.MinStorageRequiredInMB; 
            }
        }

        /// <summary>
        /// Returns a text that describing the different statuses of the filesystem.
        /// </summary>
        public string StatusString
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(Enable ? "Enabled" : "Disabled");
                status.Append(" | ");
                status.Append(Online ? "Online" : "Offline");
                status.Append(" | ");
                status.Append(Readable ? "Readable" : "Not Readable");
                status.Append(" | ");
                status.Append(Writeable ? "Writable" : "Not Writable");
                status.Append(" | ");
                status.Append(Full 
                                ? String.Format("Full (Min Req: {0} MB)", Settings.Default.MinStorageRequiredInMB) 
                                : String.Format("{0} Available", ByteCountFormatter.Format((ulong)FreeBytes)));

                return status.ToString();
            }
        }

        /// <summary>
        /// Returns the number of bytes available in the filesystem.
        /// </summary>
        public float FreeBytes
        {
            get { return _freeBytes; }
        }

        /// <summary>
        /// Returns the number of bytes below the <see cref="Filesystem.HighWatermark"/>
        /// </summary>
        /// <remarks>
        /// If the filesystem is above high watermark, <see cref="HighwaterMarkMargin"/> will become negative
        /// </remarks>
        public float HighwaterMarkMargin
        {
            get
            {
                return (_totalBytes*(float) _filesystem.HighWatermark / 100.0f) - (_totalBytes*UsedSpacePercentage/100.0f);
            }
        }


        public float TotalBytes
        {
            get { return _totalBytes; }
        }
        public float UsedSpacePercentage
        {
            get { return ((_totalBytes - _freeBytes) / _totalBytes) * 100.0F; }
        }

        public float BytesToRemove
        {
            get
            {
                float desiredUsedBytes = (((float) Filesystem.LowWatermark)/100.0f)*TotalBytes;

                return (TotalBytes - FreeBytes) - desiredUsedBytes;
            }
        }


        /// <summary>
        /// Is the filesystem above the low watermark?
        /// </summary>
        public bool AboveLowWatermark
        {
            get
            {
                return (UsedSpacePercentage > (float)Filesystem.LowWatermark);
            }
        }

        /// <summary>
        /// Is the filesystem above the high watermark?
        /// </summary>
        public bool AboveHighWatermark
        {
            get
            {
                return (UsedSpacePercentage > (float)Filesystem.HighWatermark);
            }
        }

        public bool Enable
        {
            get
            {
                return _filesystem.Enabled;
            }
        }

        #endregion


        public ServerFilesystemInfo(ServerFilesystemInfo copy)
        {
            _filesystem = copy.Filesystem;
            _online = copy.Online;
            _freeBytes = copy.FreeBytes;
            _totalBytes = copy.TotalBytes;
        }

        public string ResolveAbsolutePath(string relativePath)
        {
            return _filesystem.GetAbsolutePath(relativePath);
        } 

        internal ServerFilesystemInfo(Filesystem filesystem)
        {
            _filesystem = filesystem;
            _online = true;
            LoadFreeSpace();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        internal void LoadFreeSpace()
        {
            long freeBytesForUser;
            long totalBytes;
            long freeBytes;

            lock (_lock)
            {
                if (false == GetDiskFreeSpaceEx(_filesystem.FilesystemPath, out freeBytesForUser, out totalBytes, out freeBytes))
                {
                    // Only log ever 30 minutes.
                    if (_lastOfflineLog.AddMinutes(30) < Platform.Time)
                    {
                        Platform.Log(LogLevel.Error, "Filesystem {0} is offline.  Unable to retrieve free disk space.", _filesystem.Description);
                        _lastOfflineLog = Platform.Time;
                    }
                    _online = false;
                }
                else
                {
                    if (!_online)
                    {
                        Platform.Log(LogLevel.Error, "Filesystem {0} has gone back online.", _filesystem.Description);
                        _online = true;
                    }
                }

                _totalBytes = totalBytes;
                _freeBytes = freeBytes;
            }
        }

        
    }

    
}
