using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public class WorklistItemQueryBuilder : QueryBuilderBase, IWorklistItemQueryBuilder
	{
		private static readonly HqlJoin[] WorklistJoins = {
                HqlConstants.JoinProcedure,
                HqlConstants.JoinProcedureType,
                HqlConstants.JoinProcedureCheckIn,
                HqlConstants.JoinOrder,
                HqlConstants.JoinDiagnosticService,
                HqlConstants.JoinVisit,
                HqlConstants.JoinPatient,
                HqlConstants.JoinPatientProfile
              };

		#region Overrides of QueryBuilderBase

		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			var procedureStepClasses = args.ProcedureStepClasses;

			// if we have 1 procedure step class, we can say "from x"
			// otherwise we need to say "from ProcedureStep where ps.class = ..."
			if (procedureStepClasses.Length == 1)
			{
				var procStepClass = CollectionUtils.FirstElement(procedureStepClasses);
				query.Froms.Add(new HqlFrom(procStepClass.Name, "ps", WorklistJoins));
			}
			else
			{
				query.Froms.Add(new HqlFrom(typeof(ProcedureStep).Name, "ps", WorklistJoins));
				query.Conditions.Add(HqlCondition.IsOfClass("ps", procedureStepClasses));
			}

			// constrain patient profile to performing facility
			query.Conditions.Add(HqlConstants.ConditionConstrainPatientProfile);
		}

		#endregion

		#region Implementation of IWorklistQueryBuilder

		public virtual void AddFilters(HqlProjectionQuery query, WorklistQueryArgs args)
		{
			QueryBuilderHelpers.AddCriteriaToQuery(HqlConstants.WorklistItemQualifier, args.FilterCriteria, query, RemapHqlExpression);
		}

		public void AddActiveProcedureStepConstraint(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Conditions.Add(HqlConstants.ConditionActiveProcedureStep);
		}

		#endregion

	}
}
