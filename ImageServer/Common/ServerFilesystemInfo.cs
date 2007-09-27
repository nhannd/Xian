using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common
{
    public class ServerFilesystemInfo
    {
        #region Private Members
        private readonly Filesystem _filesystem;
        private readonly FilesystemTier _tier;
        private object _lock = new object();
        private float _freeBytes;
        private float _totalBytes;
        private bool _online;
        private DateTime _lastOfflineLog = Platform.Time.AddDays(-1);
        #endregion

        #region Public Properties
        public Filesystem Filesystem
        {
            get { return _filesystem; }
        }

        public FilesystemTier FilesystemTier
        {
            get { return _tier; }
        }

        public bool Online
        {
            get { return _online; }
        }

        public bool Writeable
        {
            get
            {
                if (!_online || _filesystem.ReadOnly || !_filesystem.Enabled)
                    return false;
                return true;
            }
        }

        public bool Readable
        {
            get
            {
                if (!_online || _filesystem.WriteOnly || !_filesystem.Enabled)
                    return false;
                return true;
            }
        }

        public float FreeBytes
        {
            get { return _freeBytes; }
        }

        public float TotalBytes
        {
            get { return _totalBytes; }
        }

        public float UsedSpacePercentage
        {
            get { return ((_totalBytes - _freeBytes) / _totalBytes) * 100.0F; }
        }

        #endregion

        internal ServerFilesystemInfo(Filesystem filesystem, FilesystemTier tier)
        {
            _filesystem = filesystem;
            _tier = tier;
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
