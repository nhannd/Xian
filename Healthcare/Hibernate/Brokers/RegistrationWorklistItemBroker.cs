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

        private static readonly HqlSelect[] WorklistItemProjection
            = {
                  SelectOrder,
                  SelectPatient,
                  SelectPatientProfile,
                  SelectMrn,
                  SelectPatientName,
                  SelectAccessionNumber,
                  SelectPriority,
                  SelectPatientClass,
                  SelectDiagnosticServiceName,
                  SelectOrderScheduledStartTime,
                  SelectHealthcard,
                  SelectDateOfBirth,
                  SelectSex
              };

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
            // note: use of the page here is somewhat meaningless given the way 2 result sets are being combined
            // may need to think about this some more
            HqlProjectionQuery orderQuery = new HqlProjectionQuery(WorklistItemFrom, WorklistItemProjection);
            orderQuery.Page = page;
            BuildOrderSearchQuery(orderQuery, where, activeOrdersOnly);
            List<WorklistItem> orders = DoQuery(orderQuery);

            HqlProjectionQuery patientQuery = new HqlProjectionQuery(PatientFrom, PatientItemProjection);
            patientQuery.Page = page;
            BuildPatientSearchQuery(patientQuery, where, workingFacility);
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
            BuildOrderSearchQuery(orderQuery, where, activeOrdersOnly);
            int orderCount = DoQueryCount(orderQuery);

            HqlProjectionQuery patientQuery = new HqlProjectionQuery(PatientFrom, PatientCountProjection);
            BuildPatientSearchQuery(patientQuery, where, workingFacility);
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
        /// <param name="procedureStepClass"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method to customize the query or return an entirely different query.
        /// </remarks>
        protected override HqlProjectionQuery CreateWorklistItemQuery(Type procedureStepClass)
        {
            HqlProjectionQuery query = new HqlProjectionQuery(GetFromClause(procedureStepClass), WorklistItemProjection);
            query.SelectDistinct = true;
            return query;
        }

        /// <summary>
        /// Creates an <see cref="HqlProjectionQuery"/> that queries for the count of worklist items based on the specified
        /// procedure-step class.
        /// </summary>
        /// <param name="procedureStepClass"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method to customize the query or return an entirely different query.
        /// </remarks>
        protected override HqlProjectionQuery CreateWorklistCountQuery(Type procedureStepClass)
        {
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

        private void BuildOrderSearchQuery(HqlQuery query, IEnumerable<WorklistItemSearchCriteria> where, bool showActiveOnly)
        {
            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("(o.Status in (?, ?))", OrderStatus.SC, OrderStatus.IP));
            }

            AddWorklistCriteria(query, where, true);
        }

        private void BuildPatientSearchQuery(HqlQuery query, IEnumerable<WorklistItemSearchCriteria> where, Facility workingFacility)
        {
            // add the criteria, but do not attempt to constrain the patient profile, as we do this differently in this case (see below)
            AddWorklistCriteria(query, where, false);

            // constrain patient profile to the working facility
            query.Conditions.Add(new HqlCondition("pp.Mrn.AssigningAuthority = ?", workingFacility.InformationAuthority));
        }

        #endregion

    }
}
