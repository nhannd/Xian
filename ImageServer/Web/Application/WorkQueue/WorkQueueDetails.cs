#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
    /// <summary>
    /// Detailed view of a <see cref="WorkQueue"/> item in the context of WorkQueue configuration.
    /// </summary>
    /// <remarks>
    /// A <see cref="WorkQueueDetails"/> is hierchical structure containing detailed information 
    /// of a <see cref="WorkQueue"/> and related data.
    /// It is an analogy to SQL "View" that the data for the view is pulled from different entities. 
    /// <para>
    /// Contained within a <see cref="WorkQueueDetails"/> is a <see cref="StudyDetails"/> which holds the information about
    /// the related study.
    /// </para>
    /// <para>
    /// A <see cref="WorkQueueDetails"/> can be created using a <see cref="WorkQueueDetailsAssembler"/> object.
    /// </para>
    /// </remarks>
    /// <seealso cref="WorkQueueSummary"/>
    /// <seealso cref="WorkQueueDetailsPanel"/>
    /// <seealso cref="StudyDetails"/>
    public class WorkQueueDetails
    {
        #region Private members
        private ServerEntityKey _ref;
        private DateTime _scheduledDateTime;
        private DateTime _expirationTime;
        private int     _failureCount;
        private WorkQueueTypeEnum _type;
        private WorkQueueStatusEnum _status;
        private string _processorID;
        private int _numSeriesPending;
        private int _numInstancesPending;

        private StudyDetails _study;

        #endregion Private members

        #region Public Properties
        public DateTime ScheduledDateTime
        {
            get { return _scheduledDateTime; }
            set { _scheduledDateTime = value; }
        }

        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
            set { _expirationTime = value; }
        }

        public int FailureCount
        {
            get { return _failureCount; }
            set { _failureCount = value; }
        }

        public WorkQueueTypeEnum Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public WorkQueueStatusEnum Status
        {
            get { return _status; }
            set { _status = value; }
        }


        public StudyDetails Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public string ProcessorID
        {
            get { return _processorID; }
            set { _processorID = value; }
        }


        public int NumInstancesPending
        {
            get { return _numInstancesPending; }
            set { _numInstancesPending = value; }
        }

        public int NumSeriesPending
        {
            get { return _numSeriesPending; }
            set { _numSeriesPending = value; }
        }

        public ServerEntityKey GUID
        {
            get { return _ref; }
            set { _ref = value; }
        }

        #endregion Public Properties
    }
}
