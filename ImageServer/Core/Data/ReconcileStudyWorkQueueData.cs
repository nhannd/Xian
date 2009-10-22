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
using ClearCanvas.Dicom;

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
	/// Represents the information encoded in the <see cref="Model.StudyIntegrityQueue.QueueData"/> column of a <see cref="Model.StudyIntegrityQueue"/> record.
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
    /// Represents the information encoded in the <see cref="Model.StudyIntegrityQueue.QueueData"/> column of a <see cref="Model.StudyIntegrityQueue"/> record.
    /// </summary>
    public class DuplicateSIQQueueData : ReconcileStudyWorkQueueData
    {
    	public List<DicomAttributeComparisonResult> ComparisonResults { get; set; }
    }
}