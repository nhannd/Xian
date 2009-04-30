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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    /// <summary>
    /// Abstract base class for brokers that evaluate worklists.
    /// </summary>
    /// <typeparam name="TItem">Class of worklist item returned by this broker.</typeparam>
    /// <remarks>
    /// <para>
    /// This class provides the basis functionality for worklist brokers.  Subclasses will typically need
    /// to override some virtual methods in order to customize the queries that are generated.  The most
    /// common methods that need to be overridden are <see cref="CreateBaseCountQuery"/> and 
    /// <see cref="CreateBaseItemQuery"/>.  Other methods may be overridden but this should not typically be necessary.
    /// </para>
    /// <para>
    /// The ability for subclasses to customize the queries relies on using an established set of HQL alias-variables
    /// to represent entities that are typically used in the query.  These are listed here:
    /// <list type="">
    /// <item>ps - ProcedureStep</item>
    /// <item>rp - Procedure</item>
    /// <item>o - Order</item>
    /// <item>p - Patient</item>
    /// <item>v - Visit</item>
    /// <item>pp - PatientProfile</item>
    /// <item>ds - DiagnosticService</item>
    /// <item>rpt - ProcedureType</item>
    /// <item>pr - Protocol</item>
	/// <item>sst - Scheduled Performer Staff</item>
	/// <item>pst - Performer Staff</item>
	/// </list>
    /// Subclasses may define additional variables but must not attempt to override those defined by this class.
    /// </para>
    /// </remarks>
    public abstract class WorklistItemBrokerBase<TItem> : Broker, IWorklistItemBroker<TItem>
		where TItem : WorklistItemBase
    {
        #region IWorklistItemSearchContext implementation

        class SearchContext : IWorklistItemSearchContext<TItem>
        {
            private WorklistItemBrokerBase<TItem> _owner;
            private WorklistItemSearchCriteria[] _searchCriteria;
            private bool _includeDegenerate;
            private int _threshold;

            public SearchContext(WorklistItemBrokerBase<TItem> owner, WorklistItemSearchCriteria[] criteria,
                bool includeDegenerate, int threshold)
            {
                _owner = owner;
                _searchCriteria = criteria;
                _includeDegenerate = includeDegenerate;
                _threshold = threshold;
            }

            public bool IncludeDegenerate
            {
                get { return _includeDegenerate; }
            }

            public WorklistItemSearchCriteria[] SearchCriteria
            {
                get { return _searchCriteria; }
            }

            public int Threshold
            {
                get { return _threshold; }
            }

            public HqlProjectionQuery BuildWorklistItemSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery)
            {
                return _owner.BuildWorklistItemSearchQuery(where, countQuery);
            }

            public HqlProjectionQuery BuildProcedureSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery)
            {
                return _owner.BuildProcedureSearchQuery(where, countQuery);
            }

            public HqlProjectionQuery BuildPatientSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery)
            {
                return _owner.BuildPatientSearchQuery(where, countQuery);
            }

            public List<TItem> DoQuery(HqlQuery query)
            {
                return _owner.DoQuery(query);
            }

            public int DoQueryCount(HqlQuery query)
            {
                return _owner.DoQueryCount(query);
            }
        }

        #endregion


        #region Hql Constants

        protected static readonly HqlSelect SelectProcedureStep = new HqlSelect("ps");
        protected static readonly HqlSelect SelectProcedure = new HqlSelect("rp");
        protected static readonly HqlSelect SelectOrder = new HqlSelect("o");
        protected static readonly HqlSelect SelectPatient = new HqlSelect("p");
        protected static readonly HqlSelect SelectPatientProfile = new HqlSelect("pp");
        protected static readonly HqlSelect SelectMrn = new HqlSelect("pp.Mrn");
        protected static readonly HqlSelect SelectPatientName = new HqlSelect("pp.Name");
        protected static readonly HqlSelect SelectAccessionNumber = new HqlSelect("o.AccessionNumber");
        protected static readonly HqlSelect SelectPriority = new HqlSelect("o.Priority");
        protected static readonly HqlSelect SelectPatientClass = new HqlSelect("v.PatientClass");
        protected static readonly HqlSelect SelectDiagnosticServiceName = new HqlSelect("ds.Name");
        protected static readonly HqlSelect SelectProcedureTypeName = new HqlSelect("rpt.Name");
        protected static readonly HqlSelect SelectProcedureStepState = new HqlSelect("ps.State");
        protected static readonly HqlSelect SelectHealthcard = new HqlSelect("pp.Healthcard");
        protected static readonly HqlSelect SelectDateOfBirth = new HqlSelect("pp.DateOfBirth");
        protected static readonly HqlSelect SelectSex = new HqlSelect("pp.Sex");

        protected static readonly HqlSelect SelectOrderScheduledStartTime = new HqlSelect("o.ScheduledStartTime");
        protected static readonly HqlSelect SelectOrderSchedulingRequestTime = new HqlSelect("o.SchedulingRequestTime");
		protected static readonly HqlSelect SelectProcedurePortable = new HqlSelect("rp.Portable");
		protected static readonly HqlSelect SelectProcedureLaterality = new HqlSelect("rp.Laterality");
        protected static readonly HqlSelect SelectProcedureScheduledStartTime = new HqlSelect("rp.ScheduledStartTime");
        protected static readonly HqlSelect SelectProcedureCheckInTime = new HqlSelect("rp.ProcedureCheckIn.CheckInTime");
        protected static readonly HqlSelect SelectProcedureCheckOutTime = new HqlSelect("rp.ProcedureCheckIn.CheckOutTime");
        protected static readonly HqlSelect SelectProcedureStartTime = new HqlSelect("rp.StartTime");
        protected static readonly HqlSelect SelectProcedureEndTime = new HqlSelect("rp.EndTime");
        protected static readonly HqlSelect SelectProcedureStepCreationTime = new HqlSelect("ps.CreationTime");
        protected static readonly HqlSelect SelectProcedureStepScheduledStartTime = new HqlSelect("ps.Scheduling.StartTime");
        protected static readonly HqlSelect SelectProcedureStepStartTime = new HqlSelect("ps.StartTime");
        protected static readonly HqlSelect SelectProcedureStepEndTime = new HqlSelect("ps.EndTime");
		protected static readonly HqlSelect SelectReportPartPreliminaryTime = new HqlSelect("rpp.PreliminaryTime");
		protected static readonly HqlSelect SelectReportPartCompletedTime = new HqlSelect("rpp.CompletedTime");

        protected static readonly HqlJoin JoinProcedure = new HqlJoin("ps.Procedure", "rp");
        protected static readonly HqlJoin JoinProcedureType = new HqlJoin("rp.Type", "rpt");
        protected static readonly HqlJoin JoinOrder = new HqlJoin("rp.Order", "o");
        protected static readonly HqlJoin JoinProtocol = new HqlJoin("ps.Protocol", "pr");
        protected static readonly HqlJoin JoinDiagnosticService = new HqlJoin("o.DiagnosticService", "ds");
        protected static readonly HqlJoin JoinVisit = new HqlJoin("o.Visit", "v");
        protected static readonly HqlJoin JoinPatient = new HqlJoin("o.Patient", "p");
        protected static readonly HqlJoin JoinPatientProfile = new HqlJoin("p.Profiles", "pp");

		protected static readonly HqlFrom hqlFromWorklist = new HqlFrom("Worklist", "w");

		protected static readonly HqlCondition ConditionActiveProcedureStep = new HqlCondition("(ps.State in (?, ?))", ActivityStatus.SC, ActivityStatus.IP);

        protected static readonly HqlSelect[] DefaultCountProjection
            = {
                  new HqlSelect("count(*)"),
              };

        private static readonly HqlJoin[] WorklistJoins
            = {
                JoinProcedure,
                JoinProcedureType,
                JoinOrder,
                JoinDiagnosticService,
                JoinVisit,
                JoinPatient,
                JoinPatientProfile
              };

		protected static readonly HqlSelect[] PatientSearchProjection
			= {
                  SelectPatient,
                  SelectPatientProfile,
                  SelectMrn,
                  SelectPatientName,
              };


		protected static readonly HqlFrom PatientSearchFrom = new HqlFrom("Patient", "p",
			new HqlJoin[]
                {
                    JoinPatientProfile
                });

		private static readonly HqlSelect[] ProcedureSearchProjection
			= {
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
                SelectProcedureScheduledStartTime
             };


		private static readonly HqlFrom ProcedureSearchFrom = new HqlFrom("Procedure", "rp",
			new HqlJoin[]
                {
                    JoinProcedureType,
                    JoinOrder,
                    JoinDiagnosticService,
                    JoinVisit,
                    JoinPatient,
                    JoinPatientProfile
                });

		#endregion

        #region Static Members

        /// <summary>
        /// Provides mappings from criteria "keys" to HQL expressions.
        /// </summary>
        private static readonly Dictionary<string, string> _mapCriteriaKeyToHql = new Dictionary<string, string>();

        /// <summary>
        /// Provides mappings from <see cref="WorklistTimeField"/> values to HQL expressions.
        /// </summary>
        private static readonly Dictionary<WorklistTimeField, HqlSelect> _mapTimeFieldToHqlSelect = new Dictionary<WorklistTimeField, HqlSelect>();

    	/// <summary>
        /// Class initializer.
        /// </summary>
        static WorklistItemBrokerBase()
        {
            // add a default set of mappings useful to all broker subclasses
            _mapCriteriaKeyToHql.Add("Order", "o");
            _mapCriteriaKeyToHql.Add("PatientProfile", "pp");
            _mapCriteriaKeyToHql.Add("Procedure", "rp");
            _mapCriteriaKeyToHql.Add("ProcedureStep", "ps");
            _mapCriteriaKeyToHql.Add("ProcedureCheckIn", "rp.ProcedureCheckIn");
            _mapCriteriaKeyToHql.Add("Protocol", "pr");
            _mapCriteriaKeyToHql.Add("ReportPart", "rpp");

            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.OrderSchedulingRequestTime, SelectOrderSchedulingRequestTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureScheduledStartTime, SelectProcedureScheduledStartTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureCheckInTime, SelectProcedureCheckInTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureCheckOutTime, SelectProcedureCheckOutTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureStartTime, SelectProcedureStartTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureEndTime, SelectProcedureEndTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureStepCreationTime, SelectProcedureStepCreationTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureStepScheduledStartTime, SelectProcedureStepScheduledStartTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureStepStartTime, SelectProcedureStepStartTime);
            _mapTimeFieldToHqlSelect.Add(WorklistTimeField.ProcedureStepEndTime, SelectProcedureStepEndTime);
			_mapTimeFieldToHqlSelect.Add(WorklistTimeField.ReportPartPreliminaryTime, SelectReportPartPreliminaryTime);
			_mapTimeFieldToHqlSelect.Add(WorklistTimeField.ReportPartCompletedTime, SelectReportPartCompletedTime);
       }

        #endregion

        private IWorklistItemSearchExecutionStrategy<TItem> _searchExecutionStrategy =
            new OptimizedWorklistItemSearchExecutionStrategy<TItem>();


        #region Public Members

        /// <summary>
        /// Gets the set of worklist items in the specified worklist.
        /// </summary>
        /// <param name="worklist"></param>
        /// <param name="wqc"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method but in most cases this should not be necessary.
        /// </remarks>
        public virtual IList<TItem> GetWorklistItems(Worklist worklist, IWorklistQueryContext wqc)
        {
            HqlProjectionQuery query = BuildWorklistQuery(worklist, wqc, false);
            return DoQuery(query);
        }

        /// <summary>
        /// Gets a count of the number of worklist items in the specified worklist.
        /// </summary>
        /// <param name="worklist"></param>
        /// <param name="wqc"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method but in most cases this should not be necessary.
        /// </remarks>
        public virtual int CountWorklistItems(Worklist worklist, IWorklistQueryContext wqc)
        {
            HqlProjectionQuery query = BuildWorklistQuery(worklist, wqc, true);
            return DoQueryCount(query);
        }

    	/// <summary>
    	/// Performs a search using the specified criteria.
    	/// </summary>
    	public IList<TItem> GetSearchResults(WorklistItemSearchCriteria[] where, bool includeDegenerate)
    	{
            SearchContext wisc = new SearchContext(this, where, includeDegenerate, 0);
            return _searchExecutionStrategy.GetSearchResults(wisc);
		}

    	/// <summary>
    	/// Gets an approximate count of the results that a search using the specified criteria would return.
    	/// </summary>
		public bool EstimateSearchResultsCount(WorklistItemSearchCriteria[] where, int threshold, bool includeDegenerate, out int count)
    	{
            SearchContext wisc = new SearchContext(this, where, includeDegenerate, threshold);
            return _searchExecutionStrategy.EstimateSearchResultsCount(wisc, out count);
    	}

    	#endregion

        #region Protected overridables

        /// <summary>
        /// Creates the base <see cref="HqlProjectionQuery"/> for worklist items queries, but does not apply
        /// conditions or sorting.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method to customize the query or return an entirely different query.
        /// </remarks>
        protected virtual HqlProjectionQuery CreateBaseItemQuery(WorklistItemSearchCriteria[] criteria)
        {
            Type procStepClass = CollectionUtils.FirstElement(criteria).ProcedureStepClass;
            WorklistTimeField timeField = CollectionUtils.FirstElement(criteria).TimeField;
            return new HqlProjectionQuery(new HqlFrom(procStepClass.Name, "ps", WorklistJoins), GetWorklistItemProjection(timeField));
        }

        /// <summary>
        /// Creates the base <see cref="HqlProjectionQuery"/> for worklist item count queries, but does not apply
        /// conditions.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <remarks>
        /// Subclasses may override this method to customize the query or return an entirely different query.
        /// </remarks>
        protected virtual HqlProjectionQuery CreateBaseCountQuery(WorklistItemSearchCriteria[] criteria)
        {
            Type procStepClass = CollectionUtils.FirstElement(criteria).ProcedureStepClass;
            return new HqlProjectionQuery(new HqlFrom(procStepClass.Name, "ps", WorklistJoins), DefaultCountProjection);
        }

        /// <summary>
        /// Maps the specified criteria "key" to an HQL alias or expression.
        /// </summary>
        /// <param name="criteriaKey"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        /// <remarks>
		/// This method is used by the <see cref="AddConditions"/> method to translate a set of
        /// <see cref="WorklistItemSearchCriteria"/> objects to a set of <see cref="HqlCondition"/> objects.
        /// Subclasses may override this method to provide additional mappings, and may delegate back to the
        /// base class to handle well-known mappings.
        /// </remarks>
        protected virtual bool MapCriteriaKeyToHql(string criteriaKey, out string alias)
        {
            return _mapCriteriaKeyToHql.TryGetValue(criteriaKey, out alias);
        }

        /// <summary>
        /// Maps the <see cref="WorklistTimeField"/> value to an <see cref="HqlSelect"/> object.
        /// </summary>
        /// <param name="timeField"></param>
        /// <param name="hql"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        protected virtual bool MapTimeFieldToHqlSelect(WorklistTimeField timeField, out HqlSelect hql)
        {
            return _mapTimeFieldToHqlSelect.TryGetValue(timeField, out hql);
        }

        /// <summary>
        /// Adds conditions to the specified query according to the filters defined by the specified worklist instance.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="worklist"></param>
		/// <param name="wqc"></param>
		/// <remarks>
        /// Subclasses may override this method to process any additional filters defined by the worklist subclass,
        /// but should be sure to also call the base class in order to process all filters defined by the <see cref="Worklist"/>
        /// class itself.
        /// </remarks>
        protected virtual void AddFilters(HqlProjectionQuery query, Worklist worklist, IWorklistQueryContext wqc)
        {
            // note that for all multi-valued filters, we avoid loading the collection of filter values
            // instead, the HqlCondition is structured using the "elements" function, which essentially generates a subquery
            // it is assumed that this should offer drastically better performance than if the filter value collection 
            // was loaded into memory, because it keeps the number of SQL round-trips to 1
            // however, no testing has actually been done to verify this assertion

            if (worklist.ProcedureTypeGroupFilter.IsEnabled)
            {
                query.Conditions.Add(new HqlCondition("rp.Type in ( select elements(ptg.ProcedureTypes) from ProcedureTypeGroup ptg where ptg in elements(w.ProcedureTypeGroupFilter.Values) )"));
            }
            if (worklist.FacilityFilter.IsEnabled)
            {
                HqlOr or = new HqlOr();
                or.Conditions.Add(new HqlCondition("rp.PerformingFacility in elements(w.FacilityFilter.Values)"));
                if (worklist.FacilityFilter.IncludeWorkingFacility && wqc.WorkingFacility != null)
                {
                    or.Conditions.Add(new HqlCondition("rp.PerformingFacility = ?", wqc.WorkingFacility));
                }
                query.Conditions.Add(or);
            }
            if (worklist.OrderPriorityFilter.IsEnabled)
            {
                query.Conditions.Add(new HqlCondition("o.Priority in elements(w.OrderPriorityFilter.Values)"));
            }
            if (worklist.PatientClassFilter.IsEnabled)
            {
                query.Conditions.Add(new HqlCondition("v.PatientClass in elements(w.PatientClassFilter.Values)"));
            }
			if (worklist.PatientLocationFilter.IsEnabled)
			{
				query.Conditions.Add(new HqlCondition("v.CurrentLocation in elements(w.PatientLocationFilter.Values)"));
			}
			if (worklist.OrderingPractitionerFilter.IsEnabled)
			{
				query.Conditions.Add(new HqlCondition("o.OrderingPractitioner in elements(w.OrderingPractitionerFilter.Values)"));
			}

            // if any of the above filters were applied, add a condition to specify w
            if (worklist.ProcedureTypeGroupFilter.IsEnabled || worklist.FacilityFilter.IsEnabled || worklist.OrderPriorityFilter.IsEnabled
				|| worklist.PatientClassFilter.IsEnabled || worklist.PatientLocationFilter.IsEnabled
				|| worklist.OrderingPractitionerFilter.IsEnabled)
            {
                AddWorklistCondition(worklist, query);
            }

            if(worklist.PortableFilter.IsEnabled)
            {
                query.Conditions.Add(new HqlCondition("rp.Portable = ?", worklist.PortableFilter.Value));
            }

            // note: worklist.TimeFilter is processed by the worklist class itself, and built into the criteria
        }

    	protected static void AddWorklistCondition(Worklist worklist, HqlProjectionQuery query)
    	{
			if (!query.Froms.Contains(hqlFromWorklist))
			{
				query.Froms.Add(hqlFromWorklist);
				query.Conditions.Add(new HqlCondition("w = ?", worklist));
			}
    	}

    	/// <summary>
        /// Adds conditions to the specified query according to the specified set of <see cref="WorklistItemSearchCriteria"/>
        /// objects.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="where"></param>
        /// <param name="constrainPatientProfile"></param>
		/// <param name="addOrderingClause"></param>
        /// <remarks>
        /// Subclasses may override this method to customize processing, but this should typically not be necessary.
        /// Callers should typically set <paramref name="constrainPatientProfile"/> to true, unless there is a specific
        /// reason not to constrain the patient profile.
        /// </remarks>
        protected virtual void AddConditions(HqlProjectionQuery query, IEnumerable<WorklistItemSearchCriteria> where, bool constrainPatientProfile, bool addOrderingClause)
        {
        	bool hasOrderingClause = false;

            HqlOr or = new HqlOr();
            foreach (WorklistItemSearchCriteria c in where)
            {
                HqlAnd and = new HqlAnd();
                foreach (KeyValuePair<string, SearchCriteria> kvp in c.SubCriteria)
                {
                    string alias;
                    if (MapCriteriaKeyToHql(kvp.Key, out alias))
                        and.Conditions.AddRange(HqlCondition.FromSearchCriteria(alias, kvp.Value));
                }
                if (and.Conditions.Count > 0)
                    or.Conditions.Add(and);

				if (addOrderingClause && !hasOrderingClause)
                {
                    foreach (KeyValuePair<string, SearchCriteria> kvp in c.SubCriteria)
                    {
                        string alias;
                        if (MapCriteriaKeyToHql(kvp.Key, out alias))
                        {
							query.Sorts.AddRange(HqlSort.FromSearchCriteria(alias, kvp.Value));
						}
                    }
					// use the sorting information from the first WorklistItemSearchCriteria object only
					// (the assumption is that they are all identical)
					hasOrderingClause = true;
				}
            }

            if (or.Conditions.Count > 0)
                query.Conditions.Add(or);

            if (constrainPatientProfile)
            {
                // constrain patient profile to performing facility
                query.Conditions.Add(
                    new HqlCondition("pp.Mrn.AssigningAuthority = rp.PerformingFacility.InformationAuthority"));
            }

			// modify the query to workaround some NHibernate bugs
			NHibernateBugWorkaround(query.Froms[0], query.Conditions);
        }

		/// <summary>
		/// NHibernate has a bug where criteria that de-reference properties not joined into the From clause are not
		/// always handled properly.  For example, in order to evaluate a criteria such as "ps.Scheduling.Performer.Staff.Name like ?",
		/// NHiberate will inject a theta-join on Staff into the SQL.  This works ok by itself.  However, when evaluating a criteria
		/// such as "ps.Scheduling.Performer.Staff.Name.FamilyName like ? or ps.Performer.Staff.Name.FamilyName like ?", NHibernate
		/// injects two Staff theta-joins into the SQL, which incorrectly results in a cross-join situation.
		/// This method modifies any query that has criteria on ps.Scheduling.Performer.Staff or ps.Performer.Staff,
		/// by adding in explicit joins to Staff for these objects, and then substituting the original conditions
		/// with conditions based on these joins.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="conditions"></param>
		private void NHibernateBugWorkaround(HqlFrom from, List<HqlCondition> conditions)
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				HqlCondition condition = conditions[i];
				if (condition is HqlJunction)
				{
					NHibernateBugWorkaround(from, ((HqlJunction)condition).Conditions);
				}
				else if (condition.Hql.StartsWith("ps.Scheduling.Performer.Staff"))
				{
					// add join for sst (scheduled staff) if not added
					if(!CollectionUtils.Contains(from.Joins,
						delegate(HqlJoin j) { return j.Alias == "sst"; }))
					{
						from.Joins.Add(new HqlJoin("ps.Scheduling.Performer.Staff", "sst", HqlJoinMode.Left));
					}
					
					// replace the condition with a new condition, using the joined Staff
					string newHql = condition.Hql.Replace("ps.Scheduling.Performer.Staff", "sst");
					conditions[i] = new HqlCondition(newHql, condition.Parameters);
				}
				else if (condition.Hql.StartsWith("ps.Performer.Staff"))
				{
					// add join for pst (performer staff) if not added
					if (!CollectionUtils.Contains(from.Joins,
						delegate(HqlJoin j) { return j.Alias == "pst"; }))
					{
						from.Joins.Add(new HqlJoin("ps.Performer.Staff", "pst", HqlJoinMode.Left));
					}
					// replace the condition with a new condition, using the joined Staff
					string newHql = condition.Hql.Replace("ps.Performer.Staff", "pst");
					conditions[i] = new HqlCondition(newHql, condition.Parameters);
				}
			}
		}

		/// <summary>
		/// Called by the public <see cref="GetSearchResults"/> methods to create a query for the set of active 
		/// worklist items matching the specified search criteria.
		/// </summary>
		/// <remarks>
		/// The implementor must return a query that will find active worklist items - that is, worklist items
		/// that are in a non-terminal state - or null to indicate that no worklist item query needs to be executed.
		/// </remarks>
		protected abstract HqlProjectionQuery BuildWorklistItemSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);

		#endregion

        #region Helpers

        private HqlSelect[] GetWorklistItemProjection(WorklistTimeField timeField)
		{
			HqlSelect selectTime;
			MapTimeFieldToHqlSelect(timeField, out selectTime);
			return new HqlSelect[]
                {
                    SelectProcedureStep,
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
                    selectTime
                };
		}

		private HqlProjectionQuery BuildWorklistQuery(Worklist worklist, IWorklistQueryContext wqc, bool countQuery)
        {
            WorklistItemSearchCriteria[] criteria = worklist.GetInvariantCriteria(wqc);
            HqlProjectionQuery query = countQuery ?
                CreateBaseCountQuery(criteria) : CreateBaseItemQuery(criteria);
            AddConditions(query, criteria, true, !countQuery);
            AddFilters(query, worklist, wqc);

            // add paging if not a count query
            if (!countQuery)
            {
                query.Page = wqc.Page;
            }

            return query;
        }

		private HqlProjectionQuery BuildPatientSearchQuery(IEnumerable<WorklistItemSearchCriteria> where, bool countQuery)
		{
			HqlProjectionQuery patientQuery = new HqlProjectionQuery(PatientSearchFrom, 
				countQuery ? DefaultCountProjection : PatientSearchProjection);

			// create a copy of the criteria that contains only the patient profile criteria, as none of the others are relevant
            // TODO: use SearchCriteria.Clone() !!!
            WorklistItemSearchCriteria[] patientCriteria = CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    WorklistItemSearchCriteria copy = new WorklistItemSearchCriteria();
                    // copy only the filter key
                    if (criteria.SubCriteria.ContainsKey("PatientProfile"))
                        copy.SubCriteria["PatientProfile"] = criteria.SubCriteria["PatientProfile"];

                    return copy;
                })
                .FindAll(delegate(WorklistItemSearchCriteria sc) { return !sc.IsEmpty; }) // remove any empties!
                .ToArray();


            // add the criteria, but do not attempt to constrain the patient profile
			// (since this is a patient search, the working facility must be used to constrain the profile)
			AddConditions(patientQuery, patientCriteria, false, false);

			// constrain patient profile to the working facility, if known
			//if (workingFacility != null)
			//{
			//    patientQuery.Conditions.Add(new HqlCondition("pp.Mrn.AssigningAuthority = ?", workingFacility.InformationAuthority));
			//}
			return patientQuery;
		}

		private HqlProjectionQuery BuildProcedureSearchQuery(IEnumerable<WorklistItemSearchCriteria> where, bool countQuery)
		{
			HqlProjectionQuery procedureQuery = new HqlProjectionQuery(ProcedureSearchFrom,
				countQuery ? DefaultCountProjection : ProcedureSearchProjection);
			AddConditions(procedureQuery, where, true, false);
			return procedureQuery;
		}


		protected List<TItem> DoQuery(HqlQuery query)
        {
            IList<object[]> list = ExecuteHql<object[]>(query);
            List<TItem> results = new List<TItem>();
            foreach (object[] tuple in list)
            {
                TItem item = (TItem)Activator.CreateInstance(typeof(TItem), tuple);
                results.Add(item);
            }

            return results;
        }

        protected int DoQueryCount(HqlQuery query)
        {
            return (int)ExecuteHqlUnique<long>(query);
        }


        #endregion
    }
}
