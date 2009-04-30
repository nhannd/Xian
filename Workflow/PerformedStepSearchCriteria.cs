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
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    public class PerformedStepSearchCriteria : EntitySearchCriteria<PerformedStep>
    {
 		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public PerformedStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
        public PerformedStepSearchCriteria(string key)
			:base(key)
		{
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected PerformedStepSearchCriteria(PerformedStepSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new PerformedStepSearchCriteria(this);
        }

        public ISearchCondition<PerformedStepStatus> Status
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Status"))
                {
                    this.SubCriteria["Status"] = new SearchCondition<PerformedStepStatus>("Status");
                }
                return (ISearchCondition<PerformedStepStatus>)this.SubCriteria["Status"];
            }
        }

        public ISearchCondition<DateTime?> StartTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StartTime"))
                {
                    this.SubCriteria["StartTime"] = new SearchCondition<DateTime?>("StartTime");
                }
                return (ISearchCondition<DateTime?>)this.SubCriteria["StartTime"];
            }
        }

        public ISearchCondition<DateTime?> EndTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("EndTime"))
                {
                    this.SubCriteria["EndTime"] = new SearchCondition<DateTime?>("EndTime");
                }
                return (ISearchCondition<DateTime?>)this.SubCriteria["EndTime"];
            }
        }
    }
}
