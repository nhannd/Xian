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
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
    /// <summary>
    /// Series mapping used when creating new study from images.
    /// </summary>
    public class SeriesMapping
    {
        #region Private Members

    	#endregion

		#region Constructors
		/// <summary>
		/// Constructor.  For Serialization.
		/// </summary>
		public SeriesMapping()
		{}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originalUid"></param>
		/// <param name="newUid"></param>
		public SeriesMapping(string originalUid, string newUid)
		{
			OriginalSeriesUid = originalUid;
			NewSeriesUid = newUid;
		}
    	#endregion

		#region Public Properties

    	public string OriginalSeriesUid { get; set; }

    	public string NewSeriesUid { get; set; }

    	#endregion
    }

	/// <summary>
	/// Represents the information encoded in the <see cref="StudyIntegrityQueue.Details"/> column of a <see cref="Model.StudyIntegrityQueue"/> record.
	/// </summary>
	public class ReconcileStudyWorkQueueData
	{
        #region Private members

		#endregion

        #region Public Properties

		public string StoragePath { get; set; }

		public ImageSetDetails Details { get; set; }

		public string UserId { get; set; }

		public DateTime? TimeStamp { get; set; }

		#endregion
	}

    /// <summary>
    /// Represents the information encoded in the <see cref="StudyIntegrityQueue.Details"/> column of a <see cref="Model.StudyIntegrityQueue"/> record.
    /// </summary>
    public class DuplicateSIQQueueData : ReconcileStudyWorkQueueData
    {
    	public List<DicomAttributeComparisonResult> ComparisonResults { get; set; }
    }
}