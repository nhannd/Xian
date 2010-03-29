#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Abstract base class for brokers that evaluate worklists.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class provides the basis functionality for worklist brokers.
	/// </para>
	/// </remarks>
	public abstract class WorklistItemBrokerBase : Broker, IWorklistItemBroker
	{
		#region IWorklistItemSearchContext implementation

		class SearchContext : IWorklistItemSearchContext
		{
			private readonly WorklistItemBrokerBase _owner;
			private readonly WorklistItemSearchArgs _args;
			private readonly Type _worklistItemClass;

			public SearchContext(WorklistItemBrokerBase owner, WorklistItemSearchArgs args, Type worklistItemClass)
			{
				_owner = owner;
				_args = args;
				_worklistItemClass = worklistItemClass;
			}

			public bool IncludeDegenerateProcedureItems
			{
				get { return _args.IncludeDegenerateProcedureItems; }
			}

			public bool IncludeDegeneratePatientItems
			{
				get { return _args.IncludeDegeneratePatientItems; }
			}

			public WorklistItemSearchCriteria[] SearchCriteria
			{
				get { return _args.SearchCriteria; }
			}

			public int Threshold
			{
				get { return _args.Threshold; }
			}

			public IList<WorklistItem> FindWorklistItems(WorklistItemSearchCriteria[] where)
			{
				var qbArgs = new SearchQueryArgs(_args.ProcedureStepClasses, where, _args.Projection);
				var query = _owner.BuildWorklistSearchQuery(qbArgs);

				// query may be null to signal that it should not be performed
				return query == null ? new List<WorklistItem>() :
					_owner.DoQuery(query, _worklistItemClass, _owner.WorklistItemQueryBuilder, qbArgs);
			}

			public int CountWorklistItems(WorklistItemSearchCriteria[] where)
			{
				var query = _owner.BuildWorklistSearchQuery(new SearchQueryArgs(_args.ProcedureStepClasses, where, null));
				return _owner.DoQueryCount(query);
			}

			public IList<WorklistItem> FindProcedures(WorklistItemSearchCriteria[] where)
			{
				var p = FilterProjection(_args.Projection, WorklistItemFieldLevel.Procedure);
				var qbArgs = new SearchQueryArgs((Type[])null, where, p);
				var query = _owner.BuildProcedureSearchQuery(qbArgs);
				return _owner.DoQuery(query, _worklistItemClass, _owner._procedureQueryBuilder, qbArgs);
			}

			public int CountProcedures(WorklistItemSearchCriteria[] where)
			{
				var query = _owner.BuildProcedureSearchQuery(new SearchQueryArgs((Type[])null, where, null));
				return _owner.DoQueryCount(query);
			}

			public IList<WorklistItem> FindPatients(WorklistItemSearchCriteria[] where)
			{
				var p = FilterProjection(_args.Projection, WorklistItemFieldLevel.Patient);
				var w = GetPatientCriteria(where);
				var qbArgs = new SearchQueryArgs((Type[]) null, w, p);
				var query = _owner.BuildPatientSearchQuery(qbArgs);
				return _owner.DoQuery(query, _worklistItemClass, _owner._patientQueryBuilder, qbArgs);
			}

			public int CountPatients(WorklistItemSearchCriteria[] where)
			{
				var w = GetPatientCriteria(where);
				var query = _owner.BuildPatientSearchQuery(new SearchQueryArgs((Type[])null, w, null));
				return _owner.DoQueryCount(query);
			}
		}

		#endregion

		private readonly IQueryBuilder _patientQueryBuilder;
		private readonly IQueryBuilder _procedureQueryBuilder;

		/// <summary>
		/// Constructor that uses defaults for procedure/patient search query builders.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		protected WorklistItemBrokerBase(IWorklistItemQueryBuilder worklistItemQueryBuilder)
			: this(worklistItemQueryBuilder, new ProcedureQueryBuilder(), new PatientQueryBuilder())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		/// <param name="procedureSearchQueryBuilder"></param>
		/// <param name="patientSearchQueryBuilder"></param>
		protected WorklistItemBrokerBase(IWorklistItemQueryBuilder worklistItemQueryBuilder,
			IQueryBuilder procedureSearchQueryBuilder, IQueryBuilder patientSearchQueryBuilder)
		{
			this.WorklistItemQueryBuilder = worklistItemQueryBuilder;

			_patientQueryBuilder = patientSearchQueryBuilder;
			_procedureQueryBuilder = procedureSearchQueryBuilder;
		}


		#region Public API

		/// <summary>
		/// Gets the set of worklist items in the specified worklist.
		/// </summary>
		/// <remarks>
		/// Subclasses may override this method but in most cases this should not be necessary.
		/// </remarks>
		public virtual IList<TItem> GetWorklistItems<TItem>(Worklist worklist, IWorklistQueryContext wqc)
			where TItem : WorklistItem
		{
			var args = new WorklistQueryArgs(worklist, wqc, false);
			var query = BuildWorklistQuery(args);
			return DoQuery<TItem>(query, this.WorklistItemQueryBuilder, args);
		}

		public virtual string GetWorklistItemsHql(Worklist worklist, IWorklistQueryContext wqc)
		{
			var args = new WorklistQueryArgs(worklist, wqc, false);
			var query = BuildWorklistQuery(args);
			return query.Hql;
		}

		/// <summary>
		/// Gets the set of items matching the specified criteria, returned as tuples shaped by the specified projection.
		/// </summary>
		public IList<object[]> GetWorklistItems(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			var args = new QueryBuilderArgs(procedureStepClasses, criteria, projection, page);
			var query = BuildWorklistQuery(args);
			return CollectionUtils.Map(ExecuteHql<object[]>(query), (object[] tuple) => this.WorklistItemQueryBuilder.PreProcessTuple(tuple, args));
		}

		/// <summary>
		/// Allow access to the HQL for debugging purposes only.  Obviously it does not make sense to pass HQL through the abstraction layer!
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public string GetWorklistItemsHql(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			var args = new QueryBuilderArgs(procedureStepClasses, criteria, projection, page);
			var query = BuildWorklistQuery(args);
			return query.Hql;
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
			var query = BuildWorklistQuery(new WorklistQueryArgs(worklist, wqc, true));
			return DoQueryCount(query);
		}

		/// <summary>
		/// Performs a search using the specified criteria.
		/// </summary>
		public IList<TItem> GetSearchResults<TItem>(WorklistItemSearchArgs args)
			where TItem : WorklistItem
		{
			var wisc = new SearchContext(this, args, typeof(TItem));
			IWorklistItemSearchExecutionStrategy strategy = new OptimizedWorklistItemSearchExecutionStrategy();
			var results = strategy.GetSearchResults(wisc);
			return CollectionUtils.Map(results, (WorklistItem r) => (TItem)r);
		}

		/// <summary>
		/// Gets an approximate count of the results that a search using the specified criteria would return.
		/// </summary>
		public bool EstimateSearchResultsCount(WorklistItemSearchArgs args, out int count)
		{
			var wisc = new SearchContext(this, args, null);
			IWorklistItemSearchExecutionStrategy strategy = new OptimizedWorklistItemSearchExecutionStrategy();
			return strategy.EstimateSearchResultsCount(wisc, out count);
		}

		#endregion

		#region Protected API

		protected IWorklistItemQueryBuilder WorklistItemQueryBuilder { get; private set; }

		protected IQueryBuilder PatientQueryBuilder { get { return _patientQueryBuilder; } }

		protected IQueryBuilder ProcedureQueryBuilder { get { return _procedureQueryBuilder; } }

		protected List<TItem> DoQuery<TItem>(HqlQuery query, IQueryBuilder queryBuilder, QueryBuilderArgs args)
			where TItem : WorklistItem
		{
			var results = DoQuery(query, typeof(TItem), queryBuilder, args);
			return CollectionUtils.Map(results, (WorklistItem r) => (TItem)r);
		}

		protected IList<WorklistItem> DoQuery(HqlQuery query, Type worklistItemClass, IQueryBuilder queryBuilder, QueryBuilderArgs args)
		{
			var list = ExecuteHql<object[]>(query);
			var results = new List<WorklistItem>();
			foreach (var tuple in list)
			{
				var item = CreateWorklistItem(worklistItemClass, queryBuilder.PreProcessTuple(tuple, args), args.Projection);
				results.Add(item);
			}

			return results;
		}

		protected int DoQueryCount(HqlQuery query)
		{
			return (int)ExecuteHqlUnique<long>(query);
		}

		protected HqlProjectionQuery BuildWorklistQuery(QueryBuilderArgs args)
		{
			var query = new HqlProjectionQuery();
			this.WorklistItemQueryBuilder.AddRootQuery(query, args);
			this.WorklistItemQueryBuilder.AddCriteria(query, args);

			if(args is WorklistQueryArgs)
			{
				this.WorklistItemQueryBuilder.AddFilters(query, (WorklistQueryArgs)args);
			}

			if (args.CountQuery)
			{
				this.WorklistItemQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				this.WorklistItemQueryBuilder.AddOrdering(query, args);
				this.WorklistItemQueryBuilder.AddItemProjection(query, args);
				this.WorklistItemQueryBuilder.AddPagingRestriction(query, args);
			}

			return query;
		}

		protected HqlProjectionQuery BuildWorklistSearchQuery(QueryBuilderArgs args)
		{
			var query = new HqlProjectionQuery();
			this.WorklistItemQueryBuilder.AddRootQuery(query, args);
			this.WorklistItemQueryBuilder.AddCriteria(query, args);
			this.WorklistItemQueryBuilder.AddActiveProcedureStepConstraint(query, args);

			if (args.CountQuery)
			{
				this.WorklistItemQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				this.WorklistItemQueryBuilder.AddItemProjection(query, args);
			}

			return query;
		}

		protected HqlProjectionQuery BuildPatientSearchQuery(SearchQueryArgs args)
		{
			var query = new HqlProjectionQuery();
			_patientQueryBuilder.AddRootQuery(query, null);
			_patientQueryBuilder.AddCriteria(query, args);

			if (args.CountQuery)
			{
				_patientQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				_patientQueryBuilder.AddItemProjection(query, args);
			}
			return query;
		}

		protected HqlProjectionQuery BuildProcedureSearchQuery(SearchQueryArgs args)
		{
			var query = new HqlProjectionQuery();
			_procedureQueryBuilder.AddRootQuery(query, null);
			_procedureQueryBuilder.AddCriteria(query, args);

			if (args.CountQuery)
			{
				_procedureQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				_procedureQueryBuilder.AddItemProjection(query, args);
			}
			return query;
		}

		#endregion

		private static WorklistItem CreateWorklistItem(Type worklistItemClass, object[] tuple, WorklistItemProjection projection)
		{
			var item = (WorklistItem)Activator.CreateInstance(worklistItemClass);
			item.InitializeFromTuple(projection, tuple);
			return item;
		}

		private static WorklistItemProjection FilterProjection(WorklistItemProjection projection, WorklistItemFieldLevel level)
		{
			return projection.Filter(f => level.Includes(f.Level));
		}

		private static WorklistItemSearchCriteria[] GetPatientCriteria(WorklistItemSearchCriteria[] where)
		{
			// create a copy of the criteria that contains only the patient profile criteria
			var filteredCopy = CollectionUtils.Map(where,
				(WorklistItemSearchCriteria criteria) =>
				(WorklistItemSearchCriteria)criteria.ClonePatientCriteriaOnly());

			return filteredCopy.FindAll(sc => !sc.IsEmpty) // remove any empties!
				.ToArray();
		}

	}
}
