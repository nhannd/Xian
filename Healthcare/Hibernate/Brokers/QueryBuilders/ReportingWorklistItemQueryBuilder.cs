using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public class ReportingWorklistItemQueryBuilder : WorklistItemQueryBuilder
	{
		private static readonly HqlCondition ConditionMostRecentPublicationStep = new HqlCondition(
			"(ps.Scheduling.StartTime = (select max(ps2.Scheduling.StartTime) from ProcedureStep ps2 where ps.ReportPart.Report = ps2.ReportPart.Report and ps2.State not in (?)))", ActivityStatus.DC);

		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			base.AddRootQuery(query, args);

			var from = query.Froms[0]; // added by base.AddRootQuery

			// join report/report part object, because may have criteria or projections on this object
			from.Joins.Add(HqlConstants.JoinReportPart);
			from.Joins.Add(HqlConstants.JoinReport);

			//TODO: we can really only apply this condition if there is only one procedure step class, but what about otherwise?
			if (args.ProcedureStepClasses.Length == 1 && CollectionUtils.FirstElement(args.ProcedureStepClasses) == typeof(PublicationStep))
				query.Conditions.Add(ConditionMostRecentPublicationStep);
		}
	}
}
