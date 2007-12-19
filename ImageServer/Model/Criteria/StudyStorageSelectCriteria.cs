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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    /// <summary>
    /// Criteria for sleects against the <see cref="StudyStorageLocation"/> table.
    /// </summary>
    public class StudyStorageSelectCriteria : SelectCriteria
    {
        public StudyStorageSelectCriteria()
            : base("StudyStorage")
        {
        }

        public ISearchCondition<ServerEntityKey> Key
        {
            get
            {
                if (!SubCriteria.ContainsKey("Key"))
                {
                    SubCriteria["Key"] = new SearchCondition<ServerEntityKey>("Key");
                }
                return (ISearchCondition<ServerEntityKey>) SubCriteria["Key"];
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
                return (ISearchCondition<ServerEntityKey>) SubCriteria["ServerPartitionKey"];
            }
        }

        public ISearchCondition<string> StudyInstanceUid
        {
            get
            {
                if (!SubCriteria.ContainsKey("StudyInstanceUid"))
                {
                    SubCriteria["StudyInstanceUid"] = new SearchCondition<string>("StudyInstanceUid");
                }
                return (ISearchCondition<string>) SubCriteria["StudyInstanceUid"];
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
                return (ISearchCondition<DateTime>) SubCriteria["InsertTime"];
            }
        }

        public ISearchCondition<DateTime> LastAccessedTime
        {
            get
            {
                if (!SubCriteria.ContainsKey("LastAccessedTime"))
                {
                    SubCriteria["LastAccessedTime"] = new SearchCondition<DateTime>("LastAccessedTime");
                }
                return (ISearchCondition<DateTime>) SubCriteria["LastAccessedTime"];
            }
        }

        public ISearchCondition<ServerEnum> StudyStatusEnum
        {
            get
            {
                if (!SubCriteria.ContainsKey("StudyStatusEnum"))
                {
                    SubCriteria["StudyStatusEnum"] = new SearchCondition<ServerEnum>("StudyStatusEnum");
                }
                return (ISearchCondition<ServerEnum>) SubCriteria["StudyStatusEnum"];
            }
        }

        public ISearchCondition<bool> Lock
        {
            get
            {
                if (!SubCriteria.ContainsKey("Lock"))
                {
                    SubCriteria["Lock"] = new SearchCondition<bool>("Lock");
                }
                return (ISearchCondition<bool>) SubCriteria["Lock"];
            }
        }
    }
}
