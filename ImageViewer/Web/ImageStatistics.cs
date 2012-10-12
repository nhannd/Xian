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
    public class AverageImageStatistics : StatisticsSet
    {
        public AverageByteCountStatistics AverageImageSize
        {
            get
            {
                if (this["AverageImageSize"] == null)
                    this["AverageImageSize"] = new AverageByteCountStatistics("AverageImageSize");

                return (this["AverageImageSize"] as AverageByteCountStatistics);
            }
            set { this["AverageImageSize"] = value; }
        }

        public AverageTimeSpanStatistics AverageSaveTime
        {
            get
            {
                if (this["AverageSaveTime"] == null)
                    this["AverageSaveTime"] = new AverageTimeSpanStatistics("AverageSaveTime");

                return (this["AverageSaveTime"] as AverageTimeSpanStatistics);
            }
            set { this["AverageSaveTime"] = value; }
        }

        public AverageTimeSpanStatistics AverageDrawToBitmapTime
        {
            get
            {
                if (this["AverageDrawToBitmapTime"] == null)
                    this["AverageDrawToBitmapTime"] = new AverageTimeSpanStatistics("AverageDrawToBitmapTime");

                return (this["AverageDrawToBitmapTime"] as AverageTimeSpanStatistics);
            }
            set { this["AverageDrawToBitmapTime"] = value; }
        }
    }

    public class ImageStatistics : StatisticsSet
	{
		#region Constructors

		public ImageStatistics(string name)
			: base("Image", name)
		{
		}

		#endregion Constructors

		#region Public Properties

        public ByteCountStatistics ImageSize
		{
			get
			{
				if (this["ImageSize"] == null)
				{
					this["ImageSize"] = new ByteCountStatistics("ImageSize");
				}
				return (this["ImageSize"] as ByteCountStatistics);
			}
			set
			{
				this["ImageSize"] = new ByteCountStatistics("ImageSize");
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
	}
}