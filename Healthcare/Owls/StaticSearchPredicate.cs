#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Default implementation of <see cref="ISearchPredicate"/> and <see cref="ISearchPredicate{TEntity,TSearchCriteria}"/>
	/// that is static (that is, time-invariant).
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TSearchCriteria"></typeparam>
	public class StaticSearchPredicate<TEntity, TSearchCriteria> : ISearchPredicate, ISearchPredicate<TEntity, TSearchCriteria>
		where TEntity : Entity
		where TSearchCriteria : EntitySearchCriteria
	{
		private readonly TSearchCriteria _viewItemCriteria;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="criteria"></param>
		public StaticSearchPredicate(TSearchCriteria criteria)
		{
			_viewItemCriteria = criteria;
		}

		/// <summary>
		/// Gets the search criteria that define this predicate.
		/// </summary>
		/// <returns></returns>
		public TSearchCriteria[] GetSearchCriteria()
		{
			return new[] { (TSearchCriteria)_viewItemCriteria.Clone() };
		}

		/// <summary>
		/// Tests this predicate against the specified instance, returning true if the instance satisfies the predicate.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public bool Test(TEntity instance)
		{
			return _viewItemCriteria.AsPredicate<TEntity>()(instance);
		}

		#region ISearchPredicate Members

		/// <summary>
		/// Gets the search criteria that define this predicate, which may be a function of the current time.
		/// </summary>
		/// <returns></returns>
		EntitySearchCriteria[] ISearchPredicate.GetSearchCriteria()
		{
			return GetSearchCriteria();
		}

		/// <summary>
		/// Tests this predicate against the specified instance, returning true if the instance satisfies the predicate.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		bool ISearchPredicate.Test(Entity instance)
		{
			return Test((TEntity) instance);
		}

		#endregion
	}
}
