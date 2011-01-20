#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Stores statistics of a WorkQueue instance processing.
	/// </summary>
	public class UpdateStudyStatistics : StatisticsSet
	{
		#region Constructors

		public UpdateStudyStatistics(string studyInstanceUid)
			: this("UpdateStudy", studyInstanceUid)
		{ }

		public UpdateStudyStatistics(string name, string studyInstanceUid)
			: base(name)
		{
			AddField("StudyInstanceUid", studyInstanceUid);
		}

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

		public ulong StudySize
		{
			set
			{
				this["StudySize"] = new ByteCountStatistics("StudySize", value);
			}
			get
			{
				if (this["StudySize"] == null)
					this["StudySize"] = new ByteCountStatistics("StudySize");

				return ((ByteCountStatistics)this["StudySize"]).Value;
			}
		}

		public int InstanceCount
		{
			set
			{
				this["InstanceCount"] = new Statistics<int>("InstanceCount", value);
			}
			get
			{
				if (this["InstanceCount"] == null)
					this["InstanceCount"] = new Statistics<int>("InstanceCount");

				return ((Statistics<int>)this["InstanceCount"]).Value;
			}
		}

		#endregion Public Properties
	}
}