#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Implementation of <see cref="IQueryBuilder"/> for creating patient search queries.
	/// </summary>
	public class PatientQueryBuilder : QueryBuilderBase
	{
		private static readonly HqlFrom DefaultFrom = new HqlFrom(typeof(PatientProfile).Name,
			"pp", new[] { new HqlJoin("pp.Patient", "p", HqlJoinMode.Left) });

		#region Overrides of QueryBuilderBase

		/// <summary>
		/// Establishes the root query (the 'from' clause and any 'join' clauses).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Froms.Add(DefaultFrom);

			// do not constrain patient profile
		}

		/// <summary>
		/// Constrains the patient profile to match the performing facility.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public override void AddConstrainPatientProfile(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			// calling this method on this class would not make any sense
			throw new InvalidOperationException();
		}

		#endregion
	}
}
