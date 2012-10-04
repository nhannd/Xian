#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Abstract base class for predicates that determine the retention of view items.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TEntitySearchCriteria"></typeparam>
	public abstract class ViewRetentionPredicateBase<TEntity, TEntitySearchCriteria> : ISearchPredicate, ISearchPredicate<TEntity, TEntitySearchCriteria>
		where TEntity : Entity
		where TEntitySearchCriteria : EntitySearchCriteria, new()
	{
		private readonly Converter<TEntitySearchCriteria, ISearchCondition<DateTime?>> _retentionConditionProviderFunc;
		private readonly TimeSpan _retentionTime;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="retentionTime"></param>
		/// <param name="retentionConditionProviderFunc"></param>
		protected ViewRetentionPredicateBase(TimeSpan retentionTime,
			Converter<TEntitySearchCriteria, ISearchCondition<DateTime?>> retentionConditionProviderFunc)
		{
			_retentionTime = retentionTime;
			_retentionConditionProviderFunc = retentionConditionProviderFunc;
		}

		#region ISearchPredicate members

		/// <summary>
		/// Gets the search criteria that define this predicate, which may be a function of the current time.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <returns></returns>
		EntitySearchCriteria[] ISearchPredicate.GetSearchCriteria()
		{
			return GetCriteriaCore();
		}

		/// <summary>
		/// Tests this predicate against the specified instance, returning true if the instance satisfies the predicate.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <param name="instance"></param>
		/// <returns></returns>
		bool ISearchPredicate.Test(Entity instance)
		{
			return TestCore((TEntity)instance);
		}

		#endregion

		#region ISearchPredicate members

		/// <summary>
		/// Gets the search criteria that define this predicate.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <returns></returns>
		TEntitySearchCriteria[] ISearchPredicate<TEntity, TEntitySearchCriteria>.GetSearchCriteria()
		{
			return GetCriteriaCore();
		}

		/// <summary>
		/// Tests this predicate against the specified instance, returning true if the instance satisfies the predicate.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <param name="instance"></param>
		/// <returns></returns>
		bool ISearchPredicate<TEntity, TEntitySearchCriteria>.Test(TEntity instance)
		{
			return TestCore(instance);
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain the criteria that define this predicate.
		/// </summary>
		/// <returns></returns>
		protected abstract TEntitySearchCriteria[] GetCriteriaCore();

		/// <summary>
		/// Extracts the retention time condition from the specified criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		protected ISearchCondition<DateTime?> GetRetentionTimeCondition(TEntitySearchCriteria criteria)
		{
			return _retentionConditionProviderFunc(criteria);
		}

		/// <summary>
		/// Gets the cutoff time as a function of the current time.
		/// </summary>
		protected DateTime CutoffTime
		{
			get { return Platform.Time - _retentionTime; }
		}

		#endregion

		private bool TestCore(TEntity instance)
		{
			// true if any of the criteria are satisfied
			return CollectionUtils.Contains(GetCriteriaCore(), sc => sc.AsPredicate<TEntity>()(instance));
		}
	}

	/// <summary>
	/// Represents a predicate that determines whether an item should be included in a view.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TEntitySearchCriteria"></typeparam>
	public class ViewInclusionPredicate<TEntity, TEntitySearchCriteria> : ViewRetentionPredicateBase<TEntity, TEntitySearchCriteria>
		where TEntity : Entity
		where TEntitySearchCriteria : EntitySearchCriteria, new()
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="retentionTime"></param>
		/// <param name="retentionConditionProviderFunc"></param>
		public ViewInclusionPredicate(TimeSpan retentionTime, Converter<TEntitySearchCriteria, ISearchCondition<DateTime?>> retentionConditionProviderFunc)
			:base(retentionTime, retentionConditionProviderFunc)
		{
		}

		/// <summary>
		/// Called to obtain the criteria that define this predicate.
		/// </summary>
		/// <returns></returns>
		protected override TEntitySearchCriteria[] GetCriteriaCore()
		{
			// the criteria for inclusion in the view is that the retention time field is either null,
			var nullCriteria = new TEntitySearchCriteria();
			GetRetentionTimeCondition(nullCriteria).IsNull();

			// or younger than the cutoff time
			var recentCriteria = new TEntitySearchCriteria();
			GetRetentionTimeCondition(recentCriteria).MoreThan(CutoffTime);

			return new []{recentCriteria, nullCriteria};
		}
	}

	/// <summary>
	/// Represents a predicate that determines whether an item should be excluded from a view.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TEntitySearchCriteria"></typeparam>
	public class ViewExclusionPredicate<TEntity, TEntitySearchCriteria> : ViewRetentionPredicateBase<TEntity, TEntitySearchCriteria>
		where TEntity : Entity
		where TEntitySearchCriteria : EntitySearchCriteria, new()
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="retentionTime"></param>
		/// <param name="retentionConditionProviderFunc"></param>
		public ViewExclusionPredicate(TimeSpan retentionTime, Converter<TEntitySearchCriteria, ISearchCondition<DateTime?>> retentionConditionProviderFunc)
			: base(retentionTime, retentionConditionProviderFunc)
		{
		}


		/// <summary>
		/// Called to obtain the criteria that define this predicate.
		/// </summary>
		/// <returns></returns>
		protected override TEntitySearchCriteria[] GetCriteriaCore()
		{
			// the criteria for exclusion from the view is that the retention time field is older than the cutoff time
			var criteria = new TEntitySearchCriteria();
			GetRetentionTimeCondition(criteria).LessThan(CutoffTime);

			return new[] { criteria };
		}
	}
}
