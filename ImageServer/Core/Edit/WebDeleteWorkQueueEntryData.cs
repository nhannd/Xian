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
using System.Xml.Serialization;

namespace ClearCanvas.ImageServer.Core.Edit
{
    /// <summary>
    /// Enumerated values for different levels of deletion.
    /// </summary>
    public enum DeletionLevel
    {
        Study,
        Series
    }

    /// <summary>
    /// Enumerated values for different levels of deletion.
    /// </summary>
    public enum MoveLevel
    {
        Study,
        Series
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// </summary>
    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteWorkQueueEntryData
    {
        #region Constructors

        public WebDeleteWorkQueueEntryData()
        {
            Level = DeletionLevel.Study;
        } 
        #endregion

		#region Public Properties

    	/// <summary>
    	/// Gets the Deletion Level.
    	/// </summary>
    	public DeletionLevel Level { get; set; }

    	/// <summary>
    	/// User-specified reason for deletion.
    	/// </summary>
    	public string Reason { get; set; }

    	/// <summary>
    	/// Gets the user who entered the delete request.
    	/// </summary>
    	public string UserId { get; set; }

    	/// <summary>
    	/// Gets the timestamp when the delete request was entered.
    	/// </summary>
    	public DateTime Timestamp { get; set; }

		/// <summary>
		/// Gets the user name who entered the delete request
		/// </summary>
    	public string UserName { get; set; }

    	#endregion
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// used for study level deletion.
    /// </summary>
    [XmlRoot("WebMoveWorkQueueEntry")]
    public class WebMoveWorkQueueEntryData
    {
    	public string Reason { get; set; }

    	public MoveLevel Level { get; set; }

    	public string UserId { get; set; }

    	public DateTime Timestamp { get; set; }

    	public WebMoveWorkQueueEntryData()
        {
            Level = MoveLevel.Study;
        }
    }

    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteStudyLevelQueueData : WebDeleteWorkQueueEntryData
    {
        public WebDeleteStudyLevelQueueData()
        {
            Level = DeletionLevel.Study;
        }
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "WebDeleteStudy" WorkQueue entry
    /// used for series level deletion.
    /// </summary>
    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteSeriesLevelQueueData : WebDeleteWorkQueueEntryData
    {
        public WebDeleteSeriesLevelQueueData()
        {
            Level = DeletionLevel.Series;
        }
    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "MoveSeries" WorkQueue entry.
    /// </summary>
    [XmlRoot("WebMoveWorkQueueEntry")]
    public class WebMoveSeriesLevelQueueData : WebMoveWorkQueueEntryData
    {
        private List<string> _seriesInstanceUids;

        public List<string> SeriesInstanceUids
        {
            get { return _seriesInstanceUids; }
            set { _seriesInstanceUids = value; }
        }

    }
}