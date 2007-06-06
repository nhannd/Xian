using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Management;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
	internal sealed class DMDriveInfo
    {
		#region Private Fields

		private DriveInfo _driveInfo;
		private string _driveName;
		
		private float _highWatermark;
		private float _lowWatermark;
		private float _watermarkMinDifference;

		private object _syncLock = new object();
		private long _usedSpace;
		private long _driveSize;

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
			lock (_syncLock)
			{
				_driveSize = _driveInfo.TotalSize;
				_usedSpace = _driveSize - _driveInfo.AvailableFreeSpace;
			}
		}
    }
}
