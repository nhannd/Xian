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
