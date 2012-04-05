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

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Utility class for querying information about a disk.
	/// </summary>
	class Diskspace
	{
		/// <summary>
		/// Formats the specified number of bytes into a human-readable value with units appropriate to the scale of the number.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string FormatBytes(double bytes)
		{
			int i;
			double dblSByte = 0;
			for (i = 0; (int)(bytes / 1024) > 0; i++, bytes /= 1024)
				dblSByte = bytes / 1024.0;
			return String.Format("{0:0.0} {1}", dblSByte, Suffix[i]);
		}

		private static readonly string[] Suffix = new[] { "B", "KB", "MB", "GB", "TB" };

		private readonly DriveInfo _driveInfo;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="drive"></param>
		public Diskspace(string drive)
		{
			_driveInfo = new DriveInfo(drive);
		}

		/// <summary>
		/// Gets the total amount of space on the disk.
		/// </summary>
		public double TotalSpace
		{
			get { return _driveInfo.TotalSize; }
		}

		/// <summary>
		/// Gets the amount of space that is currently in use.
		/// </summary>
		public double UsedSpace
		{
			get { return _driveInfo.TotalSize - _driveInfo.AvailableFreeSpace; }
		}

		/// <summary>
		/// Gets the amount of space currently in use as a percentage of the total space.
		/// </summary>
		public int UsedSpacePercent
		{
			get { return (int)Math.Round(this.UsedSpace/this.TotalSpace*100.0); }
		}
	}
}
