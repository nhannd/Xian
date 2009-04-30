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
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public interface IWorklistItemSearchContext<TItem>
        where TItem : WorklistItemBase
    {
        bool IncludeDegenerateProcedureItems { get; }
		bool IncludeDegeneratePatientItems { get; }
		WorklistItemSearchCriteria[] SearchCriteria { get; }
        int Threshold { get; }

        HqlProjectionQuery BuildWorklistItemSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);
        HqlProjectionQuery BuildProcedureSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);
        HqlProjectionQuery BuildPatientSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);
        List<TItem> DoQuery(HqlQuery query);
        int DoQueryCount(HqlQuery query);
    }


    public interface IWorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
        IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc);
        bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count);
    }

    public abstract class WorklistItemSearchExecutionStrategy<TItem> : IWorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
        #region IWorklistItemSearchExecutionStrategy<TItem> Members

        public abstract IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc);

        public abstract bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count);

        #endregion

        /// <summary>
        /// Returns a new list containing all items in primary, plus any items in secondary that were not
        /// already in primary according to the specified identity provider.  The arguments are not modified.
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <param name="identityProvider"></param>
        /// <returns></returns>
        protected static List<TItem> MergeResults(List<TItem> primary, List<TItem> secondary,
            Converter<TItem, EntityRef> identityProvider)
        {
            // note that we do not modify the arguments
            List<TItem> merged = new List<TItem>(primary);
            foreach (TItem s in secondary)
            {
                if (!CollectionUtils.Contains(primary,
                     delegate(TItem p) { return identityProvider(s).Equals(identityProvider(p), true); }))
                {
                    merged.Add(s);
                }
            }
            return merged;
        }
    }
}
