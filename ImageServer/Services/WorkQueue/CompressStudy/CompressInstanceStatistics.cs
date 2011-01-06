#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CompressStudy
{


	/// <summary>
	/// Stores statistics of a WorkQueue instance processing.
	/// </summary>
	internal class CompressInstanceStatistics : StatisticsSet
	{
		#region Constructors

		public CompressInstanceStatistics()
			: this("Instance")
		{ }

		public CompressInstanceStatistics(string name)
			: base(name)
		{ }

		#endregion Constructors

		#region Public Properties

		public TimeSpanStatistics ProcessTime
		{
			get
			{
				if (this["ProcessTime"] == null)
					this["ProcessTime"] = new TimeSpanStatistics("ProcessTime");

				return (this["ProcessTime"] as TimeSpanStatistics);
			}
			set { this["ProcessTime"] = value; }
		}

		public ulong FileSize
		{
			set
			{
				this["FileSize"] = new ByteCountStatistics("FileSize", value);
			}
			get
			{
				if (this["FileSize"] == null)
					this["FileSize"] = new ByteCountStatistics("FileSize");

				return ((ByteCountStatistics) this["FileSize"]).Value;
			}
		}

		public TimeSpanStatistics FileLoadTime
		{
			get
			{
				if (this["FileLoadTime"] == null)
					this["FileLoadTime"] = new TimeSpanStatistics("FileLoadTime");

				return (this["FileLoadTime"] as TimeSpanStatistics);
			}
			set { this["FileLoadTime"] = value; }
		}

		public TimeSpanStatistics CompressTime
		{
			get
			{
				if (this["CompressTime"] == null)
					this["CompressTime"] = new TimeSpanStatistics("CompressTime");

				return (this["CompressTime"] as TimeSpanStatistics);
			}
			set { this["CompressTime"] = value; }
		}
		#endregion Public Properties
	}


}
