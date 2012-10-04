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
	/// Defines an interface to an object that acts as a predicate used to search for or test entity instances.
	/// </summary>
	public interface ISearchPredicate
	{
		/// <summary>
		/// Gets the search criteria that define this predicate, which may be a function of the current time.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <returns></returns>
		EntitySearchCriteria[] GetSearchCriteria();

		/// <summary>
		/// Tests this predicate against the specified instance, returning true if the instance satisfies the predicate.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <param name="instance"></param>
		/// <returns></returns>
		bool Test(Entity instance);
	}

	/// <summary>
	/// Defines a generic interface to an object that acts as a predicate used to search for or test entity instances.
	/// </summary>
	public interface ISearchPredicate<TEntity, TSearchCriteria>
		where TEntity : Entity
		where TSearchCriteria : EntitySearchCriteria
	{
		/// <summary>
		/// Gets the search criteria that define this predicate.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <returns></returns>
		TSearchCriteria[] GetSearchCriteria();

		/// <summary>
		/// Tests this predicate against the specified instance, returning true if the instance satisfies the predicate.
		/// </summary>
		/// <remarks>
		/// Note: this may be a function of the current time, hence calling this function on the same argument
		/// at a different point in time may yield a different result.
		/// </remarks>
		/// <param name="instance"></param>
		/// <returns></returns>
		bool Test(TEntity instance);
	}
}
