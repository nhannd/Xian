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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    /// <summary>
    /// Implementation of <see cref="IRegistrationWorklistItemBroker"/>.
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class RegistrationWorklistItemBroker : WorklistItemBrokerBase<WorklistItem>, IRegistrationWorklistItemBroker
    {
        #region HQL Constants

        private static readonly HqlSelect[] WorklistItemCount
            = {
                  new HqlSelect("count(distinct o)"),
              };


        private static readonly HqlJoin[] WorklistItemJoins
            = {
                  JoinOrder,
                  JoinDiagnosticService,
                  JoinVisit,
                  JoinPatient,
                  JoinPatientProfile
              };

        private static readonly HqlFrom WorklistItemFrom = new HqlFrom("Procedure", "rp", WorklistItemJoins);

        private static readonly HqlSelect[] PatientItemProjection
           = {
                  SelectPatient,
                  SelectPatientProfile,
                  SelectMrn,
                  SelectPatientName,
                  SelectHealthcard,
                  SelectDateOfBirth,
                  SelectSex
              };

        private static readonly HqlSelect[] PatientCountProjection
            = {
                  new HqlSelect("count(p)"),
              };

        private static readonly HqlFrom PatientFrom = new HqlFrom("Patient", "p", 
            new HqlJoin[]
                {
                    JoinPatientProfile
                });

        #endregion

        #region IRegistrationWorklistItemBroker members

        /// <summary>
        /// Performs a search using the specified criteria.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="page"></param>
        /// <param name="activeOrdersOnly"></param>
        /// <param name="workingFacility"></param>
        /// <returns></returns>
        public IList<WorklistItem> GetSearchResults(WorklistItemSearchCriteria[] where, SearchResultPage page, bool activeOrdersOnly, Facility workingFacility)
        {
            // ensure criteria are filtering on correct type of step, and display the correct time field
            // ProcedureScheduledStartTime seems like a reasonable choice for registration homepage search,
            // as it gives a general sense of when the procedure occurs in time
            CollectionUtils.ForEach(where,
                delegate(WorklistItemSearchCriteria sc)
                {
                    sc.TimeField = WorklistTimeField.ProcedureScheduledStartTime;
                });

            // note: use of the page here is somewhat meaningless given the way 2 result sets are being combined
            // may need to think about this some more
            HqlProjectionQuery orderQuery = new HqlProjectionQuery(WorklistItemFrom, GetWorklistItemProjection(WorklistTimeField.ProcedureScheduledStartTime));
            orderQuery.Page = page;
            BuildOrderSearchQuery(orderQuery, where, activeOrdersOnly, false);
            List<WorklistItem> orders = DoQuery(orderQuery);

            HqlProjectionQuery patientQuery = new HqlProjectionQuery(PatientFrom, PatientItemProjection);
            patientQuery.Page = page;
            BuildPatientSearchQuery(patientQuery, where, workingFacility, false);
            List<WorklistItem> patients = DoQuery(patientQuery);

            List<WorklistItem> results = new List<WorklistItem>(orders);
            foreach (WorklistItem patient in patients)
            {
                if(!CollectionUtils.Contains(orders, delegate (WorklistItem item) { return item.PatientRef.Equals(patient.PatientRef, true); }))
                    results.Add(patient);
            }
            return results;
        }

        /// <summary>
        /// Gets a count of the results that a search using the specified criteria would return.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="activeOrdersOnly"></param>
        /// <param name="workingFacility"></param>
        /// <returns></returns>
        public int CountSearchResultsApprox(WorklistItemSearchCriteria[] where, bool activeOrdersOnly, Facility workingFacility)
        {
            HqlProjectionQuery orderQuery = new HqlProjectionQuery(WorklistItemFrom, WorklistItemCount);
            BuildOrderSearchQuery(orderQuery, where, activeOrdersOnly, true);
            int orderCount = DoQueryCount(orderQuery);

            HqlProjectionQuery patientQuery = new HqlProjectionQuery(PatientFrom, PatientCountProjection);
            BuildPatientSearchQuery(patientQuery, where, workingFacility, true);
            int patientCount = DoQueryCount(patientQuery);

            // there is no clear way to combine these numbers, since the order count may include
            // patients that are subsequently counted in the patientCount, and conversely, a single
            // patient may have many orders

            // however, the exact number doesn't matter that much, since this method is really just
            // used to decide whether a given set of criteria is specific enough
            // returning the larger number is probably adequate for this purpose
            return Math.Max(orderCount, patientCount);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates an <see cref="HqlProjectionQuery"/> that queries for worklist items based on the specified
        /// procedure-step class.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method to customize the query or return an entirely different query.
        /// </remarks>
        protected override HqlProjectionQuery CreateBaseItemQuery(WorklistItemSearchCriteria[] criteria)
        {
            Type procedureStepClass = CollectionUtils.FirstElement(criteria).ProcedureStepClass;
            WorklistTimeField timeField = CollectionUtils.FirstElement(criteria).TimeField;
            HqlProjectionQuery query = new HqlProjectionQuery(GetFromClause(procedureStepClass), GetWorklistItemProjection(timeField));
            query.SelectDistinct = true;
            return query;
        }

        /// <summary>
        /// Creates an <see cref="HqlProjectionQuery"/> that queries for the count of worklist items based on the specified
        /// procedure-step class.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method to customize the query or return an entirely different query.
        /// </remarks>
        protected override HqlProjectionQuery CreateBaseCountQuery(WorklistItemSearchCriteria[] criteria)
        {
            Type procedureStepClass = CollectionUtils.FirstElement(criteria).ProcedureStepClass;
            return new HqlProjectionQuery(GetFromClause(procedureStepClass), WorklistItemCount);
        }

        #endregion

        #region Private Helpers

        private HqlFrom GetFromClause(Type stepClass)
        {
            if (stepClass == null)
                return WorklistItemFrom;
            else
            {
                HqlFrom from = new HqlFrom(stepClass.Name, "ps");
                from.Joins.Add(JoinProtocol);
                from.Joins.Add(JoinProcedure);
                from.Joins.AddRange(WorklistItemJoins);
                return from;
            }
        }

        private HqlSelect[] GetWorklistItemProjection(WorklistTimeField timeField)
        {
            HqlSelect selectTime;
            MapTimeFieldToHqlSelect(timeField, out selectTime);

            return new HqlSelect[]
                {
                      SelectOrder,
                      SelectPatient,
                      SelectPatientProfile,
                      SelectMrn,
                      SelectPatientName,
                      SelectAccessionNumber,
                      SelectPriority,
                      SelectPatientClass,
                      SelectDiagnosticServiceName,
                      selectTime,
                      SelectHealthcard,
                      SelectDateOfBirth,
                      SelectSex
                  };
        }

        private void BuildOrderSearchQuery(HqlQuery query, IEnumerable<WorklistItemSearchCriteria> where, bool showActiveOnly, bool countQuery)
        {
            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("(o.Status in (?, ?))", OrderStatus.SC, OrderStatus.IP));
            }

            AddConditions(query, where, true, countQuery);
        }

        private void BuildPatientSearchQuery(HqlQuery query, IEnumerable<WorklistItemSearchCriteria> where, Facility workingFacility, bool countQuery)
        {
            // create a copy of the criteria that contains only the patient profile criteria, as none of the others are relevant
            List<WorklistItemSearchCriteria> patientCriteria = CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    WorklistItemSearchCriteria copy = new WorklistItemSearchCriteria();
                    copy.SubCriteria["PatientProfile"] = criteria.PatientProfile;
                    return copy;
                });

            // add the criteria, but do not attempt to constrain the patient profile, as we do this differently in this case (see below)
            AddConditions(query, patientCriteria, false, countQuery);

            // constrain patient profile to the working facility, if known
            if (workingFacility != null)
            {
                query.Conditions.Add(
                    new HqlCondition("pp.Mrn.AssigningAuthority = ?", workingFacility.InformationAuthority));
            }
        }

        #endregion

    }
}
