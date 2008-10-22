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

        private static readonly HqlJoin[] WorklistItemJoins
            = {
                  JoinOrder,
				  JoinProcedureType,
                  JoinDiagnosticService,
                  JoinVisit,
                  JoinPatient,
                  JoinPatientProfile
              };

        private static readonly HqlFrom WorklistItemFrom = new HqlFrom("Procedure", "rp", WorklistItemJoins);

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
            return new HqlProjectionQuery(GetFromClause(procedureStepClass), GetWorklistItemProjection(timeField));
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
            return new HqlProjectionQuery(GetFromClause(procedureStepClass), DefaultCountProjection);
        }

		protected override HqlProjectionQuery BuildWorklistItemSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery)
		{
			Type procedureStepClass = CollectionUtils.FirstElement(where).ProcedureStepClass;

			// if the search is coming from the Registration folder system, the ps class will be null,
			// in which case there is no point doing a search for worklist items, because the patient/order search performed
			// by the base class will cover it
			if(procedureStepClass == null)
				return null;

			// need to display the correct time field
			// ProcedureScheduledStartTime seems like a reasonable choice for registration homepage search,
			// as it gives a general sense of when the procedure occurs in time
			CollectionUtils.ForEach(where,
				delegate(WorklistItemSearchCriteria sc)
				{
					sc.TimeField = WorklistTimeField.ProcedureScheduledStartTime;
				});

			HqlProjectionQuery query = countQuery ? CreateBaseCountQuery(where) : CreateBaseItemQuery(where);
			query.Conditions.Add(ConditionActiveProcedureStep);
			AddConditions(query, where, true, !countQuery);

			return query;
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
                      SelectProcedure,
                      SelectOrder,
                      SelectPatient,
                      SelectPatientProfile,
                      SelectMrn,
                      SelectPatientName,
                      SelectAccessionNumber,
                      SelectPriority,
                      SelectPatientClass,
                      SelectDiagnosticServiceName,
					  SelectProcedureTypeName,
					  SelectProcedurePortable,
					  SelectProcedureLaterality,
                      selectTime,
                      SelectHealthcard,
                      SelectDateOfBirth,
                      SelectSex
                  };
        }

        #endregion

    }
}
