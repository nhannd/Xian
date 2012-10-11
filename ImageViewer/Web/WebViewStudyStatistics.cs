#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageViewer.Web
{
	public class WebViewStudyStatistics : StatisticsSet
	{
		#region Constructors

		public WebViewStudyStatistics(string name)
			: base("WebViewStudy", name)
		{
		}

		#endregion Constructors

		#region Public Properties

		public ulong ImageSize
		{
			get
			{
				if (this["ImageSize"] == null)
				{
					this["ImageSize"] = new ByteCountStatistics("ImageSize");
				}
				return (this["ImageSize"] as ByteCountStatistics).Value;
			}
			set
			{
				this["ImageSize"] = new ByteCountStatistics("ImageSize", value);
			}
		}

		public TimeSpanStatistics SaveTime
		{
			get
			{
				if (this["SaveTime"] == null)
				{
					this["SaveTime"] = new TimeSpanStatistics("SaveTime");
				}
				return this["SaveTime"] as TimeSpanStatistics;
			}
			set
			{
				this["SaveTime"] = value;
			}
		}

		public TimeSpanStatistics DrawToBitmapTime
		{
			get
			{
				if (this["DrawToBitmapTime"] == null)
				{
					this["DrawToBitmapTime"] = new TimeSpanStatistics("DrawToBitmapTime");
				}
				return this["DrawToBitmapTime"] as TimeSpanStatistics;
			}
			set
			{
				this["DrawToBitmapTime"] = value;
			}
		}

		#endregion Public Properties

		#region Public Methods


		#endregion Public Methods
	}
}