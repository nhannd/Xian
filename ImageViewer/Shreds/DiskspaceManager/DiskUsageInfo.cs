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
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
	internal sealed class DiskUsageInfo
	{
		#region Private Fields

		private readonly DriveInfo _driveInfo;

		private volatile float _highWatermark;
		private volatile float _lowWatermark;
		private volatile float _watermarkMinDifference;

		private readonly object _syncLock = new object();
		private long _usedSpace;
		private long _diskSize;
		private bool _refreshing = false;

		#endregion

		public DiskUsageInfo(string name)
		{
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo info in drives)
			{
				string driveName = info.Name.TrimEnd(new[] {Platform.PathSeparator});
				if (String.Compare(driveName, name, true) == 0)
				{
					_driveInfo = info;
					break;
				}
			}

			if (_driveInfo == null)
				throw new ArgumentException(String.Format(SR.ExceptionDriveIsNotValid, name));

			DiskName = name;
			Refresh();

			_highWatermark = DiskspaceManagerSettings.HighWaterMarkDefault;
			_lowWatermark = DiskspaceManagerSettings.LowWaterMarkDefault;
			_watermarkMinDifference = 5;
		}

		/// <summary>
		/// Gets the name of the disk.
		/// </summary>
		public string DiskName { get; private set; }

		/// <summary>
		/// Gets or sets the high watermark as a percentage of total disk space (i.e. 5.0 = 5%).
		/// </summary>
		public float HighWatermark
		{
			get { return _highWatermark; }
			set { _highWatermark = FixRange(value, LowWatermark + WatermarkMinDifference, 100); }
		}

		/// <summary>
		/// Gets or sets the low watermark as a percentage of total disk space (i.e. 5.0 = 5%).
		/// </summary>
		public float LowWatermark
		{
			get { return _lowWatermark; }
			set { _lowWatermark = FixRange(value, 0, HighWatermark - WatermarkMinDifference); }
		}

		/// <summary>
		/// Gets or sets the minimum percentage difference between the low and high watermarks (i.e. 5.0 = 5%).
		/// </summary>
		public float WatermarkMinDifference
		{
			get { return _watermarkMinDifference; }
			set { _watermarkMinDifference = FixRange(value, 0, 100); }
		}

		/// <summary>
		/// Gets the disk usage over the high watermark in bytes.
		/// </summary>
		public long BytesOverHighWatermark
		{
			get { return UsedSpace - (long) (HighWatermark/100F*DiskSize); }
		}

		/// <summary>
		/// Gets the disk usage over the low watermark in bytes.
		/// </summary>
		public long BytesOverLowWatermark
		{
			get { return UsedSpace - (long) (LowWatermark/100F*DiskSize); }
		}

		/// <summary>
		/// Gets the total disk size in bytes.
		/// </summary>
		public long DiskSize
		{
			get
			{
				lock (_syncLock)
				{
					return _diskSize;
				}
			}
		}

		/// <summary>
		/// Gets the total disk usage in bytes.
		/// </summary>
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

		/// <summary>
		/// Gets the disk usage as a percentage of the total disk size (i.e. 5.0 = 5%).
		/// </summary>
		public float UsedSpacePercentage
		{
			get { return UsedSpace/((float) DiskSize)*100.0F; }
		}

		/// <summary>
		/// Sets both the low and high watermarks as a percentage of total disk space (i.e. 5.0 = 5%) in a single atomic operation.
		/// </summary>
		/// <param name="lowWatermark">The low watermark as a percentage of total disk space.</param>
		/// <param name="highWatermark">The high watermark as a percentage of total disk space.</param>
		public void SetWatermarks(float lowWatermark, float highWatermark)
		{
			// by performing fix on both new values, we guarantee that highWM - lowWM > deltaWM and that both are in the range [0,100]
			// while having these restrictions based on the new values and not the old ones
			highWatermark = FixRange(highWatermark, lowWatermark + WatermarkMinDifference, 100);
			lowWatermark = FixRange(lowWatermark, 0, highWatermark - WatermarkMinDifference);

			// assign only after all range fixes have been carried out
			_highWatermark = highWatermark;
			_lowWatermark = lowWatermark;
		}

		/// <summary>
		/// Refreshes the disk usage statistics.
		/// </summary>
		public void Refresh()
		{
			Refresh(Timeout.Infinite);
		}

		/// <summary>
		/// Refreshes the disk usage statistics.
		/// </summary>
		/// <param name="waitMilliseconds">Minimum time, in milliseconds, to wait before the operation times out.</param>
		public void Refresh(int waitMilliseconds)
		{
			lock (_syncLock)
			{
				if (!_refreshing)
				{
					_refreshing = true;
					ThreadPool.QueueUserWorkItem(PerformRefresh, null);
				}

				Monitor.Wait(_syncLock, waitMilliseconds);
			}
		}

		private void PerformRefresh(object nothing)
		{
			lock (_syncLock)
			{
				try
				{
					//these can take a while.
					_diskSize = _driveInfo.TotalSize;
					_usedSpace = _diskSize - _driveInfo.AvailableFreeSpace;
				}
				finally
				{
					_refreshing = false;
					Monitor.PulseAll(_syncLock);
				}
			}
		}

		private static float FixRange(float input, float minValue, float maxValue)
		{
			if (input < minValue)
				return minValue;
			if (input > maxValue)
				return maxValue;
			return input;
		}
	}
}