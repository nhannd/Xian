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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using System;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services
{

    public class WorklistItemTextQueryHelper<TDomainItem, TSummary>
        : TextQueryHelper<TDomainItem, WorklistItemSearchCriteria, TSummary>
        where TDomainItem : WorklistItemBase
        where TSummary : DataContractBase
    {
		class TextQueryCriteria : WorklistItemSearchCriteria
		{
			private readonly bool _includeDegeneratePatientItems;
			private readonly bool _includeDegenerateProcedureItems;

			public TextQueryCriteria(WorklistItemSearchCriteria that, bool includeDegeneratePatientItems, bool includeDegenerateProcedureItems)
				:base(that)
			{
				_includeDegeneratePatientItems = includeDegeneratePatientItems;
				_includeDegenerateProcedureItems = includeDegenerateProcedureItems;
			}

			public bool IncludeDegeneratePatientItems
			{
				get { return _includeDegeneratePatientItems; }
			}

			public bool IncludeDegenerateProcedureItems
			{
				get { return _includeDegenerateProcedureItems; }
			}
		}


    	private readonly Type _procedureStepClass;
    	private readonly IWorklistItemBroker<TDomainItem> _broker;
    	private readonly WorklistItemTextQueryOptions _options;
    	private readonly IPersistenceContext _context;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistItemTextQueryHelper(
			IWorklistItemBroker<TDomainItem> broker,
			Converter<TDomainItem, TSummary> summaryAssembler,
			Type procedureStepClass,
			WorklistItemTextQueryOptions options,
			IPersistenceContext context)
			: base(null, summaryAssembler, null, null)
		{
			_broker = broker;
			_procedureStepClass = procedureStepClass;
			_options = options;
			_context = context;
		}

		#region Overrides

        protected override bool ValidateRequest(TextQueryRequest request)
        {
			// if the UseAdvancedSearch flag is set, check if the Search fields are empty
			WorklistItemTextQueryRequest req = (WorklistItemTextQueryRequest)request;
			if (req.UseAdvancedSearch)
			{
				return req.SearchFields != null && !req.SearchFields.IsEmpty();
			}

			// otherwise, do base behaviour (check text query)
			return base.ValidateRequest(request);
        }

        protected override WorklistItemSearchCriteria[] BuildCriteria(TextQueryRequest request)
        {
            WorklistItemTextQueryRequest req = (WorklistItemTextQueryRequest)request;
			List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

			if ((_options & WorklistItemTextQueryOptions.PatientOrder) == WorklistItemTextQueryOptions.PatientOrder)
			{
                criteria.AddRange(BuildPatientOrderSearchCriteria(req));
			}

			if ((_options & WorklistItemTextQueryOptions.ProcedureStepStaff) == WorklistItemTextQueryOptions.ProcedureStepStaff)
			{
                criteria.AddRange(BuildStaffSearchCriteria(req));
			}

			// add constraint for downtime vs live procedures
			bool downtimeRecoveryMode = (_options & WorklistItemTextQueryOptions.DowntimeRecovery) ==
										WorklistItemTextQueryOptions.DowntimeRecovery;
			criteria.ForEach(delegate(WorklistItemSearchCriteria c) { c.Procedure.DowntimeRecoveryMode.EqualTo(downtimeRecoveryMode); });

			// this is a silly hack to append additional information (degenerate flags) into the criteria so that we can
			// pass them on to the TestSpecificity and DoQuery methods
        	List<WorklistItemSearchCriteria> augmented = CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(
        		criteria,
        		delegate(WorklistItemSearchCriteria c)
        		{
					return new TextQueryCriteria(c,
						ShouldIncludeDegeneratePatientItems(req),
						ShouldIncludeDegenerateProcedureItems(req));
        		});

			return augmented.ToArray();
		}

		protected override bool TestSpecificity(WorklistItemSearchCriteria[] where, int threshold)
		{
			TextQueryCriteria c = (TextQueryCriteria) CollectionUtils.FirstElement(where);
			WorklistItemSearchArgs searchArgs = new WorklistItemSearchArgs(
				where,
				c.IncludeDegeneratePatientItems,
				c.IncludeDegenerateProcedureItems,
				threshold);

			int count;
			return _broker.EstimateSearchResultsCount(searchArgs, out count);
		}

		protected override IList<TDomainItem> DoQuery(WorklistItemSearchCriteria[] where, SearchResultPage page)
		{
			TextQueryCriteria c = (TextQueryCriteria)CollectionUtils.FirstElement(where);
			WorklistItemSearchArgs searchArgs = new WorklistItemSearchArgs(
				where,
				c.IncludeDegeneratePatientItems,
				c.IncludeDegenerateProcedureItems);

			return _broker.GetSearchResults(searchArgs);
		}

		#endregion

		#region Patient Criteria builders

		private List<WorklistItemSearchCriteria> BuildPatientOrderSearchCriteria(WorklistItemTextQueryRequest request)
        {
			if(request.UseAdvancedSearch)
			{
				return BuildAdvancedPatientOrderSearchCriteria(request);
			}
			else
			{
				return BuildAdHocPatientOrderSearchCriteria(request);
			}
        	
        }

		private List<WorklistItemSearchCriteria> BuildAdvancedPatientOrderSearchCriteria(WorklistItemTextQueryRequest request)
		{
			Platform.CheckMemberIsSet(request.SearchFields, "SearchFields");

			WorklistItemTextQueryRequest.AdvancedSearchFields searchParams = request.SearchFields;


			List<WorklistItemSearchCriteria> wheres = new List<WorklistItemSearchCriteria>();

			// construct a base criteria object from the request values
			WorklistItemSearchCriteria criteria = new WorklistItemSearchCriteria(_procedureStepClass);

			ApplyStringValue(criteria.PatientProfile.Mrn.Id, searchParams.Mrn);
			ApplyStringValue(criteria.PatientProfile.Name.FamilyName, searchParams.FamilyName);
			ApplyStringValue(criteria.PatientProfile.Name.GivenName, searchParams.GivenName);
			ApplyStringValue(criteria.PatientProfile.Healthcard.Id, searchParams.HealthcardNumber);
			ApplyStringValue(criteria.Order.AccessionNumber, searchParams.AccessionNumber);

			if (searchParams.OrderingPractitionerRef != null)
			{
				ExternalPractitioner orderedBy = _context.Load<ExternalPractitioner>(searchParams.OrderingPractitionerRef, EntityLoadFlags.Proxy);
				criteria.Order.OrderingPractitioner.EqualTo(orderedBy);
			}

			if (searchParams.ProcedureTypeRef != null)
			{
				ProcedureType pt = _context.Load<ProcedureType>(searchParams.ProcedureTypeRef, EntityLoadFlags.Proxy);
				criteria.Procedure.Type.EqualTo(pt);
			}

			if(searchParams.FromDate != null || searchParams.UntilDate != null)
			{
				// the goal here is to use the date-range in an approximate fashion, to search for procedures
				// that were performed "around" that time-frame
				// therefore, the date-range is applied to muliple dates, and these are OR'd

				// use "day" resolution on the start and end times, because we don't care about time
				WorklistTimePoint start = searchParams.FromDate == null ? null
					: new WorklistTimePoint(searchParams.FromDate.Value, WorklistTimePoint.Resolutions.Day);
				WorklistTimePoint end = searchParams.UntilDate == null ? null
					: new WorklistTimePoint(searchParams.UntilDate.Value, WorklistTimePoint.Resolutions.Day);

				WorklistTimeRange dateRange = new WorklistTimeRange(start, end);
				DateTime now = Platform.Time;

				WorklistItemSearchCriteria procSchedDateCriteria = (WorklistItemSearchCriteria)criteria.Clone();
				dateRange.Apply((ISearchCondition)procSchedDateCriteria.Procedure.ScheduledStartTime, now);
				wheres.Add(procSchedDateCriteria);

				WorklistItemSearchCriteria procStartDateCriteria = (WorklistItemSearchCriteria)criteria.Clone();
				dateRange.Apply((ISearchCondition)procStartDateCriteria.Procedure.StartTime, now);
				wheres.Add(procStartDateCriteria);

				WorklistItemSearchCriteria procEndDateCriteria = (WorklistItemSearchCriteria)criteria.Clone();
				dateRange.Apply((ISearchCondition)procEndDateCriteria.Procedure.EndTime, now);
				wheres.Add(procEndDateCriteria);

			}
			else
			{
				// no date range, so just need a single criteria
				wheres.Add(criteria);
			}

			return wheres;
		}

        private List<WorklistItemSearchCriteria> BuildAdHocPatientOrderSearchCriteria(WorklistItemTextQueryRequest request)
		{
            string query = request.TextQuery;

			// this will hold all criteria
			List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

			// build criteria against names
			PersonName[] names = ParsePersonNames(query);
			criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
				delegate(PersonName n)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);
					sc.PatientProfile.Name.FamilyName.StartsWith(n.FamilyName);
					if (n.GivenName != null)
						sc.PatientProfile.Name.GivenName.StartsWith(n.GivenName);
					return sc;
				}));

			// build criteria against Mrn identifiers
			string[] ids = ParseIdentifiers(query);
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string word)
				{
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
					c.PatientProfile.Mrn.Id.StartsWith(word);
					return c;
				}));

			// build criteria against Healthcard identifiers
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string word)
				{
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
					c.PatientProfile.Healthcard.Id.StartsWith(word);
					return c;
				}));

			// build criteria against Accession Number
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string word)
				{
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
					c.Order.AccessionNumber.StartsWith(word);
					return c;
				}));

			return criteria;
		}

		#endregion

		#region Staff Criteria builders

		private IEnumerable<WorklistItemSearchCriteria> BuildStaffSearchCriteria(WorklistItemTextQueryRequest request)
		{
			if (request.UseAdvancedSearch)
			{
				// advanced search not supported here
				throw new NotSupportedException();
			}
			else
			{
				return BuildAdHocStaffSearchCriteria(request);
			}
		}

		private List<WorklistItemSearchCriteria> BuildAdHocStaffSearchCriteria(WorklistItemTextQueryRequest request)
		{
            string query = request.TextQuery;

            // this will hold all criteria
			List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

			// build criteria against names
			PersonName[] names = ParsePersonNames(query);

			// scheduled performer
			criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
				delegate(PersonName n)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);

					PersonNameSearchCriteria scheduledPerformerNameCriteria = sc.ProcedureStep.Scheduling.Performer.Staff.Name;
					scheduledPerformerNameCriteria.FamilyName.StartsWith(n.FamilyName);
					if (n.GivenName != null)
						scheduledPerformerNameCriteria.GivenName.StartsWith(n.GivenName);
					return sc;
				}));

			// actual performer
			criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
				delegate(PersonName n)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);

					PersonNameSearchCriteria performerNameCriteria = sc.ProcedureStep.Performer.Staff.Name;
					performerNameCriteria.FamilyName.StartsWith(n.FamilyName);
					if (n.GivenName != null)
						performerNameCriteria.GivenName.StartsWith(n.GivenName);
					return sc;
				}));

			// build criteria against Staff ID identifiers
			// bug #3952: use ParseTerms instead of ParseIdentifiers, because a Staff ID might only contain letters
			string[] ids = ParseTerms(query);

			// scheduled performer
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string id)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);
					sc.ProcedureStep.Scheduling.Performer.Staff.Id.StartsWith(id);
					return sc;
				}));

			// actual performer
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string id)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);
					sc.ProcedureStep.Performer.Staff.Id.StartsWith(id);
					return sc;
				}));

			return criteria;
		}

		#endregion

		private bool ShouldIncludeDegenerateProcedureItems(WorklistItemTextQueryRequest request)
		{
			// generally, if the search query is being used on patients/orders, then it makes sense to include
			// degenerate procedure items
			// conversely, if this flag is not present, then including degenerate items could result in an open query
			// on the entire database which would obviously not be desirable
			return (_options & WorklistItemTextQueryOptions.PatientOrder) == WorklistItemTextQueryOptions.PatientOrder;
		}

		private bool ShouldIncludeDegeneratePatientItems(WorklistItemTextQueryRequest request)
		{
			// include degenerate patient items iff
			// 1) degen procedure items are being included, and
			// 2) advanced search is not being used, or it is being used and all non-patient search criteria are empty
			return ShouldIncludeDegenerateProcedureItems(request)
			       && (!request.UseAdvancedSearch || request.SearchFields.IsNonPatientFieldsEmpty());
		}

		/// <summary>
		/// Applies specified string value to specified condition, if the value is non-empty.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="value"></param>
		private static void ApplyStringValue(ISearchCondition<string> condition, string value)
		{
			if(value != null)
			{
				string trimmed = value.Trim();
				if (!string.IsNullOrEmpty(trimmed))
				{
					condition.StartsWith(trimmed);
				}
			}
		}

	}
}