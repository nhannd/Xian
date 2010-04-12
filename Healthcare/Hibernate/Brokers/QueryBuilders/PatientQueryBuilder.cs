using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Implementation of <see cref="IQueryBuilder"/> for creating patient search queries.
	/// </summary>
	public class PatientQueryBuilder : QueryBuilderBase
	{
		private static readonly HqlFrom DefaultFrom = new HqlFrom(typeof(Patient).Name,
			"p", new[] { HqlConstants.JoinPatientProfile });

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

		#endregion
	}
}
