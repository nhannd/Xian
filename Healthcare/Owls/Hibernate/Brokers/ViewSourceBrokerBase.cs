#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;
using ClearCanvas.Healthcare.Owls.Brokers;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Abstract base implementation of <see cref="IViewSourceBroker{TViewItem,TRootEntity}"/>.
	/// </summary>
	/// <typeparam name="TViewItem"></typeparam>
	/// <typeparam name="TRootEntity"></typeparam>
	public abstract class ViewSourceBrokerBase<TViewItem, TRootEntity> : Broker, IViewSourceBroker<TViewItem, TRootEntity>
		where TViewItem : Entity,  new()
		where TRootEntity : Entity
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="queryBuilder"></param>
		/// <param name="viewSourceProjection"></param>
		protected ViewSourceBrokerBase(IQueryBuilder queryBuilder, WorklistItemProjection viewSourceProjection)
		{
			this.QueryBuilder = queryBuilder;
			this.SourceProjection = viewSourceProjection;
		}

		#region IViewSourceBroker members

		/// <summary>
		/// Creates view items from source data for the specified root entity instance, used to incrementally update the view.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public virtual IList<TViewItem> GetViewItems(TRootEntity instance)
		{
			var where = new WorklistItemSearchCriteria();
			var subCriteria = GetRootEntitySubCriteria(where);
			subCriteria.OID.EqualTo(instance.OID);

			return QueryViewItems(new Type[0], where, false);
		}

		/// <summary>
		/// Populates the view from source data, as a single long-running (non-transactional) step.
		/// </summary>
		/// <param name="args"></param>
		public void PopulateView(PopulateViewArgs args)
		{
			// here we use the SqlBulkCopy functionality to achieve the best possible insert performance

			// initialize the data reader
			object bookmark = null;
			var reader = new ViewItemDataReader(this.Configuration, typeof(TViewItem),
							delegate
							{
								// since this is a long-lived operation, we regularly clear the session cache
								// to free up memory 
								this.ClearSessionCache();

								// load next batch of items (bookmark keeps track of position)
								var items = GetViewItemBatchHelper(args.RootEntityClass, ref bookmark, args.ViewInclusionPredicate, args.ReadBatchSize);
								return CollectionUtils.ToArray(items);
							}) { AutoGenerateObjectIDs = true };

			// Set up the bulk copy object. 
			// Note that the column positions in the source
			// data reader match the column positions in 
			// the destination table so there is no need to
			// map columns.
			using (var bulkCopy = new SqlBulkCopy(this.ConnectionString))
			{
				bulkCopy.DestinationTableName = reader.TableName;
				bulkCopy.BulkCopyTimeout = (int)args.Timeout.TotalSeconds;
				bulkCopy.BatchSize = args.WriteBatchSize;
				bulkCopy.NotifyAfter = args.WriteBatchSize;
				bulkCopy.SqlRowsCopied += ((sender, e) => args.ProgressCallback(new PopulateViewProgress(e.RowsCopied)));

				// ensure reader is disposed after use
				using(reader)
				{
					// Write from the source to the destination.
					bulkCopy.WriteToServer(reader);
				}
			}
		}

		#endregion


		#region Protected API

		/// <summary>
		/// Gets the sub-criteria corresponding to the root entity.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		protected abstract EntitySearchCriteria GetRootEntitySubCriteria(WorklistItemSearchCriteria criteria);

		/// <summary>
		/// Returns true if the view contains a PatientProfile and Procedure such that performing facility is 
		/// of the same information authority as the patient profile.
		/// </summary>
		/// <param name="viewItem"></param>
		/// <returns></returns>
		protected abstract bool IsPatientProfileAligned(TViewItem viewItem);

		/// <summary>
		/// Gets the tuple mapping function for the specified projection.
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		protected abstract TupleMappingDelegate GetTupleMapping(WorklistItemProjection projection);

		#endregion

		#region Helpers


		/// <summary>
		/// Gets or sets the query builder used by this broker to build queries.
		/// </summary>
		private IQueryBuilder QueryBuilder { get; set; }

		/// <summary>
		/// Gets or sets the projection used by this broker to shape results.
		/// </summary>
		private WorklistItemProjection SourceProjection { get; set; }

		/// <summary>
		/// Loads the next batch of view items from source data, beginning at the specified bookmark.
		/// The bookmark is updated.
		/// </summary>
		/// <param name="rootEntityClass"></param>
		/// <param name="bookmark"></param>
		/// <param name="inclusionPredicate"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		private IEnumerable<TViewItem> GetViewItemBatchHelper(Type rootEntityClass, ref object bookmark, ISearchPredicate inclusionPredicate, int batchSize)
		{
			// for performance reasons, we do this in two steps:
			// 1. Determine the next batch of root entities to be processed. This can be done using a simple index seek
			// on the clustered index (OID) or the root entity, which is very fast.
			// 2. Build the set of view items from source, by limiting the root entity OID to the set returned in (1).
			// We do this second query without constraining the patient profile, which allows it to run fast,
			// and then filter out the "bad" rows.

			// first we get the set of OIDs of the next batch of root entity instances to be processed
			var criteria = inclusionPredicate.GetSearchCriteria();
			var oids = GetOIDBatch(rootEntityClass, batchSize, ref bookmark, criteria);

			// if there are no OIDs, there are no more view items to generate
			if (oids.Count == 0)
				return new List<TViewItem>();

			// construct a WorklistItemSearchCriteria where we limit the root entity OID to being in the set returned above
			var wiCriteria = new WorklistItemSearchCriteria();
			var subCriteria = GetRootEntitySubCriteria(wiCriteria);
			subCriteria.OID.In(oids);

			// query for view items from source
			var psClasses = new[] { rootEntityClass };
			return QueryViewItems(psClasses, wiCriteria, true);
		}

		/// <summary>
		/// Gets the set of OIDs of the next batch of root entities to be processed, beginning at the specified bookmark.
		/// The bookmark is updated.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <param name="batchSize"></param>
		/// <param name="bookmark"></param>
		/// <param name="criteria"></param>
		/// <returns></returns>
		private IList<object> GetOIDBatch(Type entityClass, int batchSize, ref object bookmark, SearchCriteria[] criteria)
		{
			var query = new HqlQuery(string.Format("select x.OID from {0} x", entityClass.Name));
			query.Sorts.Add(new HqlSort("x.OID", true, 0));
			query.Page = new SearchResultPage(0, batchSize);

			// add criteria
			var or = new HqlOr(CollectionUtils.Map<SearchCriteria, HqlCondition>(criteria,
				sc => new HqlAnd(HqlCondition.FromSearchCriteria("x", sc))));
			query.Conditions.Add(or);
			
			// add bookmark criteria
			if (bookmark != null)
			{
				query.Conditions.Add(HqlCondition.MoreThan("x.OID", bookmark));
			}

			var oids = ExecuteHql<object>(query);

			// update bookmark
			bookmark = CollectionUtils.LastElement(oids);
			return oids;
		}

		/// <summary>
		/// Execute query for view items from source, based on specified criteria.
		/// </summary>
		/// <param name="psClasses"></param>
		/// <param name="where"></param>
		/// <param name="expectManyResults"></param>
		/// <returns></returns>
		private IList<TViewItem> QueryViewItems(Type[] psClasses, WorklistItemSearchCriteria where, bool expectManyResults)
		{
			var criteria = new[] { where };
			var args = new QueryBuilderArgs(psClasses, criteria, this.SourceProjection, new SearchResultPage());

			// for performance reasons, we don't constrain the patient profile if we expect many rows
			// instead, we filter out the "bad" rows after the fact
			var query = BuildQuery(args, !expectManyResults);
			var results = DoQuery(query, this.QueryBuilder, args);

			if (expectManyResults)
			{
				// filter out the bad rows
				return CollectionUtils.Select(results, IsPatientProfileAligned);
			}
			return results;
		}

		/// <summary>
		/// Builds a worklist item query, including the ordering and paging directives.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="constrainPatientProfile"></param>
		/// <returns></returns>
		private HqlProjectionQuery BuildQuery(QueryBuilderArgs args, bool constrainPatientProfile)
		{
			var query = new HqlProjectionQuery();
			this.QueryBuilder.AddRootQuery(query, args);
			this.QueryBuilder.AddCriteria(query, args);
			this.QueryBuilder.AddOrdering(query, args);
			this.QueryBuilder.AddItemProjection(query, args);
			this.QueryBuilder.AddPagingRestriction(query, args);

			if (constrainPatientProfile)
			{
				this.QueryBuilder.AddConstrainPatientProfile(query, args);
			}

			return query;
		}

		/// <summary>
		/// Executes the specified query, using the specified query-builder to pre-process the results.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="queryBuilder"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private IList<TViewItem> DoQuery(HqlQuery query, IQueryBuilder queryBuilder, QueryBuilderArgs args)
		{
			// do query
			var tuples = ExecuteHql<object[]>(query);

			// get tuple mapping exactly once
			var tupleMapping = GetTupleMapping(args.Projection);

			// use tuple mapping to create worklist items
			return CollectionUtils.Map<object[], TViewItem>(tuples,
				tuple => CreateWorklistItem(queryBuilder.PreProcessResult(tuple, args), tupleMapping));
		}


		/// <summary>
		/// Creates a worklist item from the specified tuple.
		/// </summary>
		private TViewItem CreateWorklistItem(object[] tuple, TupleMappingDelegate tupleMapping)
		{
			var item = new TViewItem();
			tupleMapping(item, tuple, this.Context);
			return item;
		}

		#endregion

	}
}
