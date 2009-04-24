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
