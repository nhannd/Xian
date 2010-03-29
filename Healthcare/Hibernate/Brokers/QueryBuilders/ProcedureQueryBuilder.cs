using System;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public class ProcedureQueryBuilder : QueryBuilderBase
	{
		private static readonly HqlFrom DefaultFrom = new HqlFrom(typeof(Procedure).Name, "rp", new[]
                {
                    HqlConstants.JoinProcedureType,
                    HqlConstants.JoinOrder,
                    HqlConstants.JoinDiagnosticService,
                    HqlConstants.JoinVisit,
                    HqlConstants.JoinPatient,
                    HqlConstants.JoinPatientProfile
                });

		#region Overrides of QueryBuilderBase<SearchQueryArgs>

		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Froms.Add(DefaultFrom);

			// constrain patient profile to performing facility
			query.Conditions.Add(HqlConstants.ConditionConstrainPatientProfile);
		}

		#endregion


	}
}
