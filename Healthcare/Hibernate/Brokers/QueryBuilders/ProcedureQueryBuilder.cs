using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Implementation of <see cref="IQueryBuilder"/> for creating procedure search queries.
	/// </summary>
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

		#region Overrides of QueryBuilderBase

		/// <summary>
		/// Establishes the root query (the 'from' clause and any 'join' clauses).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Froms.Add(DefaultFrom);
		}

		/// <summary>
		/// Constrains the patient profile to match the performing facility.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public override void AddConstrainPatientProfile(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			// constrain patient profile to performing facility
			query.Conditions.Add(HqlConstants.ConditionConstrainPatientProfile);
		}

		#endregion
	}
}
