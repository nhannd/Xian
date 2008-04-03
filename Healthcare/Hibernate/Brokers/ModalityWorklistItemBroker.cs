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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    /// <summary>
    /// Implementation of <see cref="IModalityWorklistItemBroker"/>.
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ModalityWorklistItemBroker : WorklistItemBrokerBase<WorklistItem>, IModalityWorklistItemBroker
    {
        #region IModalityWorklistItemBroker Members

        /// <summary>
        /// Performs a search for modality worklist items using the specified criteria.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="page"></param>
        /// <param name="showActiveOnly"></param>
        /// <returns></returns>
        public IList<WorklistItem> GetSearchResults(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            // ensure criteria are filtering on correct type of step, and display the correct time field
            // ProcedureScheduledStartTime seems like a reasonable choice for tech homepage search,
            // as it gives a general sense of when the procedure occurs in time, regardless of the procedure step
            CollectionUtils.ForEach(where,
                delegate(WorklistItemSearchCriteria sc)
                {
                    sc.ProcedureStepClass = typeof(ModalityProcedureStep);
                    sc.TimeField = WorklistTimeField.ProcedureScheduledStartTime;
                });
            HqlProjectionQuery query = CreateBaseItemQuery(where);
            query.Page = page;
            BuildSearchQuery(query, where, showActiveOnly, false);
            return DoQuery(query);
        }

        /// <summary>
        /// Obtains a count of the number of results that a search using the specified criteria would return.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="showActiveOnly"></param>
        /// <returns></returns>
        public int CountSearchResults(WorklistItemSearchCriteria[] where, bool showActiveOnly)
        {
            // ensure criteria are filtering on correct type of step
            CollectionUtils.ForEach(where,
                delegate(WorklistItemSearchCriteria sc) { sc.ProcedureStepClass = typeof(ModalityProcedureStep); });
            HqlProjectionQuery query = CreateBaseCountQuery(where);
            BuildSearchQuery(query, where, showActiveOnly, true);
            return DoQueryCount(query);
        }

        #endregion

        #region Private Helpers

        private void BuildSearchQuery(HqlQuery query, IEnumerable<WorklistItemSearchCriteria> where, bool showActiveOnly, bool countQuery)
        {
            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("ps.State in (?, ?)", ActivityStatus.SC, ActivityStatus.IP));
            }

            AddConditions(query, where, true, countQuery);
        }

        #endregion
    }
}
