#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class WorkQueueSelectCriteria : SelectCriteria
    {
        public WorkQueueSelectCriteria()
            : base("WorkQueue")
        {}

        public ISearchCondition<ServerEntityKey> Key
        {
            get
            {
                if (!SubCriteria.ContainsKey("Key"))
                {
                    SubCriteria["Key"] = new SearchCondition<ServerEntityKey>("Key");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["Key"];
            }
        }
        public ISearchCondition<ServerEntityKey> StudyStorageKey
        {
            get
            {
                if (!SubCriteria.ContainsKey("StudyStorageKey"))
                {
                    SubCriteria["StudyStorageKey"] = new SearchCondition<ServerEntityKey>("StudyStorageKey");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["StudyStorageKey"];
            }
        }
        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
                if (!SubCriteria.ContainsKey("ServerPartitionKey"))
                {
                    SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["ServerPartitionKey"];
            }
        }
        public ISearchCondition<ServerEntityKey> DeviceKey
        {
            get
            {
                if (!SubCriteria.ContainsKey("DeviceKey"))
                {
                    SubCriteria["DeviceKey"] = new SearchCondition<ServerEntityKey>("DeviceKey");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["DeviceKey"];
            }
        }
        public ISearchCondition<WorkQueueTypeEnum> WorkQueueTypeEnum
        {
            get
            {
                if (!SubCriteria.ContainsKey("WorkQueueTypeEnum"))
                {
                    SubCriteria["WorkQueueTypeEnum"] = new SearchCondition<WorkQueueTypeEnum>("WorkQueueTypeEnum");
                }
                return (ISearchCondition<WorkQueueTypeEnum>)SubCriteria["WorkQueueTypeEnum"];
            }
        }

        public ISearchCondition<WorkQueueStatusEnum> WorkQueueStatusEnum
        {
            get
            {
                if (!SubCriteria.ContainsKey("WorkQueueStatusEnum"))
                {
                    SubCriteria["WorkQueueStatusEnum"] = new SearchCondition<WorkQueueStatusEnum>("WorkQueueStatusEnum");
                }
                return (ISearchCondition<WorkQueueStatusEnum>)SubCriteria["WorkQueueStatusEnum"];
            }
        }
        public ISearchCondition<DateTime> ExpirationTime
        {
            get
            {
                if (!SubCriteria.ContainsKey("ExpirationTime"))
                {
                    SubCriteria["ExpirationTime"] = new SearchCondition<DateTime>("ExpirationTime");
                }
                return (ISearchCondition<DateTime>)SubCriteria["ExpirationTime"];
            }
        }
        public ISearchCondition<DateTime> ScheduledTime
        {
            get
            {
                if (!SubCriteria.ContainsKey("ScheduledTime"))
                {
                    SubCriteria["ScheduledTime"] = new SearchCondition<DateTime>("ScheduledTime");
                }
                return (ISearchCondition<DateTime>)SubCriteria["ScheduledTime"];
            }
        }
        public ISearchCondition<DateTime> InsertTime
        {
            get
            {
                if (!SubCriteria.ContainsKey("InsertTime"))
                {
                    SubCriteria["InsertTime"] = new SearchCondition<DateTime>("InsertTime");
                }
                return (ISearchCondition<DateTime>)SubCriteria["InsertTime"];
            }
        }

        public ISearchCondition<int> FailureCount
        {
            get
            {
                if (!SubCriteria.ContainsKey("FailureCount"))
                {
                    SubCriteria["FailureCount"] = new SearchCondition<int>("FailureCount");
                }
                return (ISearchCondition<int>)SubCriteria["FailureCount"];
            }
        }
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the Series table.
        /// </summary>
        /// <remarks>
        /// A <see cref="SeriesSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="Series"/> tables is automatically added into the <see cref="SeriesSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<SelectCriteria> StudyFilesystemRelatedEntityCondition
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyStorageRelatedEntityCondition"))
                {
                    this.SubCriteria["StudyStorageRelatedEntityCondition"] = new RelatedEntityCondition<SelectCriteria>("StudyStorageRelatedEntityCondition", 
                        "StudyStorageKey","StudyStorageKey");
                }
                return (IRelatedEntityCondition<SelectCriteria>)this.SubCriteria["StudyStorageRelatedEntityCondition"];
            }
        }
    }
}
