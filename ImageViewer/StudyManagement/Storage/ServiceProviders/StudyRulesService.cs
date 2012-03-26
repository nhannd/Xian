using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
	// TODO (Marmot): This seems like the best place for this, since it has to be available (in process) whenever the database is present.
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	internal class StudyRulesServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IStudyRulesService))
				return new StudyRulesService();

			return null;
		}

		#endregion
	}

	internal class StudyRulesService : IStudyRulesService
	{
		#region Implementation of IStudyRulesService

		public ListRulesResponse ListRules(ListRulesRequest request)
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetRuleBroker();
				var rules = broker.GetRules();
				return new ListRulesResponse(rules.Select(r => r.RuleData).ToList());
			}
		}

		public AddRuleResponse AddRule(AddRuleRequest request)
		{
			using (var context = new DataAccessContext())
			{
				var rule = new Rule
							{
								RuleId = request.Rule.Id,
								Name = request.Rule.Name,
								RuleData = request.Rule
							};
				context.GetRuleBroker().AddRule(rule);
				context.Commit();

				return new AddRuleResponse();
			}
		}

		public UpdateRuleResponse UpdateRule(UpdateRuleRequest request)
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetRuleBroker();
				var rule = broker.GetRule(request.Rule.Id);
				rule.RuleData = request.Rule;
				context.Commit();

				return new UpdateRuleResponse();
			}
		}

		public DeleteRuleResponse DeleteRule(DeleteRuleRequest request)
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetRuleBroker();
				broker.DeleteRule(request.RuleId);
				context.Commit();

				return new DeleteRuleResponse();
			}
		}

		#endregion
	}
}
