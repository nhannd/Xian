#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Management;
using System.Diagnostics;
using System.Threading;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
	internal sealed class DMDriveInfo
    {
		#region Private Fields

		private readonly DriveInfo _driveInfo;
		private readonly string _driveName;
		
		private volatile float _highWatermark;
		private volatile float _lowWatermark;
		private volatile float _watermarkMinDifference;

		private readonly object _syncLock = new object();
		private long _usedSpace;
		private long _driveSize;
		private bool _refreshing = false;
		#endregion

        public DMDriveInfo(string name)
        {
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo info in drives)
			{
				string driveName = info.Name.TrimEnd(new char[] { Platform.PathSeparator });
				if (String.Compare(driveName, name, true) == 0)
				{
					_driveInfo = info;
					break;
				}
			}

			if (_driveInfo == null)
				throw new ArgumentException(String.Format(SR.ExceptionDriveIsNotValid, name));

			_driveName = name;
			this.Refresh();

			_highWatermark = DiskspaceManagerSettings.HighWaterMarkDefault;
            _lowWatermark = DiskspaceManagerSettings.LowWaterMarkDefault;
            _watermarkMinDifference = 5;
        }

        public string DriveName
        {
			get { return _driveName; }
        }

		public long DriveSize
		{
			get
			{
				lock (_syncLock)
				{
					return _driveSize;
				}
			}
		}
		
		public float HighWatermark
        {
            get { return _highWatermark; }
            set { _highWatermark = value >= _lowWatermark + _watermarkMinDifference ? value : _lowWatermark + _watermarkMinDifference; }
        }

		public long BytesOverHighWaterMark
		{
			get
			{
				return this.UsedSpace - (long)(this.HighWatermark / 100F * this.DriveSize);
			}
		}

        public float LowWatermark
        {
            get { return _lowWatermark; }
            set { _lowWatermark = value <= _highWatermark - _watermarkMinDifference ? value : _highWatermark - _watermarkMinDifference; }
        }

		public long BytesOverLowWaterMark
		{
			get 
			{
				return this.UsedSpace - (long)(this.LowWatermark / 100F * this.DriveSize);
			}
		}

        public float WatermarkMinDifference
        {
            get { return _watermarkMinDifference; }
            set { _watermarkMinDifference = value; }
        }

		public long UsedSpace
		{
			get 
			{
				lock (_syncLock)
				{
					return _usedSpace;
				}
			}
		}

        public float UsedSpacePercentage
        {
            get { return this.UsedSpace / ((float)this.DriveSize) * 100.0F; }
        }

		public void Refresh()
		{
			Refresh(Timeout.Infinite);
		}

		public void Refresh(int waitMilliseconds)
		{
			lock (_syncLock)
			{
				if (!_refreshing)
				{
					_refreshing = true;
					ThreadPool.QueueUserWorkItem(Refresh, null);
				}

				Monitor.Wait(_syncLock, waitMilliseconds);
			}
		}

		private void Refresh(object nothing)
		{
			lock (_syncLock)
			{
				try
				{
					//these can take a while.
					_driveSize = _driveInfo.TotalSize;
					_usedSpace = _driveSize - _driveInfo.AvailableFreeSpace;
				}
				finally
				{
					_refreshing = false;
					Monitor.PulseAll(_syncLock);
				}
			}
		}
    }
}
