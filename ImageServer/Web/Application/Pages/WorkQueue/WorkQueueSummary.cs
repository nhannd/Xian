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

namespace ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue
{
    /// <summary>
    /// Summary view of a <see cref="WorkQueue"/> item in the WorkQueue configuration UI.
    /// </summary>
    /// <remarks>
    /// A <see cref="WorkQueueSummary"/> contains the summary of a <see cref="WorkQueue"/> and related information and is displayed
    /// in the WorkQueue configuration UI.
    /// <para>
    /// A <see cref="WorkQueueSummary"/> can be created using a <see cref="WorkQueueSummaryAssembler"/> object.
    /// </para>
    /// </remarks>
    public class WorkQueueSummary
    {
        #region Private members

        private ServerEntityKey _workQueueGuid;
        private string _patientID;
        private string _patientName;
        private WorkQueueTypeEnum _type;
        private WorkQueueStatusEnum _status;
        private WorkQueuePriorityEnum _priority;
        private DateTime _scheduledDateTime;
        private string _notes;
        private string _processorID;
        #endregion Private members

        #region Public Properties

        public DateTime ScheduledDateTime
        {
            get { return _scheduledDateTime; }
            set { _scheduledDateTime = value; }
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

        public string PatientID
        {
            get { return _patientID; }
            set { _patientID = value; }
        }

        public string PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public ServerEntityKey WorkQueueGuid
        {
            get { return _workQueueGuid; }
            set { _workQueueGuid = value; }
        }

        public WorkQueuePriorityEnum Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public string ProcessorID
        {
            get { return _processorID; }
            set { _processorID = value; }
        }

        #endregion Public Properties
    }
}
