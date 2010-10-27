#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Extends <see cref="IQueryBuilder"/> with functionality specific to worklist item queries.
	/// </summary>
	public interface IWorklistItemQueryBuilder : IQueryBuilder
	{
		/// <summary>
		/// Adds worklist filters to the query (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddFilters(HqlProjectionQuery query, WorklistQueryArgs args);

		/// <summary>
		/// Adds the "active procedure step" constraint (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddActiveProcedureStepConstraint(HqlProjectionQuery query, QueryBuilderArgs args);
	}
}
