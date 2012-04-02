using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	class Diskspace
	{
		public static string FormatBytes(double bytes, bool includeUnits)
		{
			int i;
			double dblSByte = 0;
			for (i = 0; (int)(bytes / 1024) > 0; i++, bytes /= 1024)
				dblSByte = bytes / 1024.0;
			return includeUnits ? String.Format("{0:0.0} {1}", dblSByte, Suffix[i])
				: String.Format("{0:0.0}", dblSByte);
		}

		private static readonly string[] Suffix = new[] { "B", "KB", "MB", "GB", "TB" };

		private readonly DriveInfo _driveInfo;

		public Diskspace(string drive)
		{
			_driveInfo = new DriveInfo(drive);
		}

		public double TotalSpace
		{
			get { return _driveInfo.TotalSize; }
		}

		public double UsedSpace
		{
			get { return _driveInfo.TotalSize - _driveInfo.AvailableFreeSpace; }
		}

		public int UsedSpacePercent
		{
			get { return (int)Math.Round(this.UsedSpace/this.TotalSpace*100.0); }
		}
	}
}
