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

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Defines a set of constants that describe time-fields used by worklists.
    /// </summary>
    public class WorklistTimeField
    {
        public static readonly WorklistTimeField OrderSchedulingRequestTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.Order.SchedulingRequestTime;
            });

        public static readonly WorklistTimeField ProcedureScheduledStartTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.Procedure.ScheduledStartTime;
            });

        public static readonly WorklistTimeField ProcedureCheckInTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.Procedure.ProcedureCheckIn.CheckInTime;
            });

        public static readonly WorklistTimeField ProcedureCheckOutTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.Procedure.ProcedureCheckIn.CheckOutTime;
            });

        public static readonly WorklistTimeField ProcedureStartTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.Procedure.StartTime;
            });

        public static readonly WorklistTimeField ProcedureEndTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.Procedure.EndTime;
            });

        public static readonly WorklistTimeField ProcedureStepCreationTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.ProcedureStep.CreationTime;
            });

        public static readonly WorklistTimeField ProcedureStepScheduledStartTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.ProcedureStep.Scheduling.StartTime;
            });

        public static readonly WorklistTimeField ProcedureStepStartTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.ProcedureStep.StartTime;
            });

        public static readonly WorklistTimeField ProcedureStepEndTime = new WorklistTimeField(
            delegate(WorklistItemSearchCriteria criteria)
            {
                return (ISearchCondition)criteria.ProcedureStep.EndTime;
            });

		public static readonly WorklistTimeField ReportPartPreliminaryTime = new WorklistTimeField(
			delegate(WorklistItemSearchCriteria criteria)
			{
				ReportingWorklistItemSearchCriteria c = (ReportingWorklistItemSearchCriteria) criteria;
				return (ISearchCondition)c.ReportPart.PreliminaryTime;
			});

		public static readonly WorklistTimeField ReportPartCompletedTime = new WorklistTimeField(
			delegate(WorklistItemSearchCriteria criteria)
			{
				ReportingWorklistItemSearchCriteria c = (ReportingWorklistItemSearchCriteria)criteria;
				return (ISearchCondition)c.ReportPart.CompletedTime;
			});


        private readonly Converter<WorklistItemSearchCriteria, ISearchCondition> _mapping;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteriaMapping"></param>
        public WorklistTimeField(Converter<WorklistItemSearchCriteria, ISearchCondition> criteriaMapping)
        {
            _mapping = criteriaMapping;
        }

        /// <summary>
        /// Returns the search-condition field on the specified criteria object representing the time-field
        /// described by this instance.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        internal ISearchCondition GetSearchCondition(WorklistItemSearchCriteria criteria)
        {
            return _mapping(criteria);
        }
    }
}
