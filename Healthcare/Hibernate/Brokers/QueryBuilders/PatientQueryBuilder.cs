using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public class PatientQueryBuilder : QueryBuilderBase
	{
		private static readonly HqlFrom DefaultFrom = new HqlFrom(typeof(Patient).Name,
			"p", new[] { HqlConstants.JoinPatientProfile });

		#region Overrides of QueryBuilderBase

		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Froms.Add(DefaultFrom);

			// do not constrain patient profile
		}

		#endregion
	}
}
