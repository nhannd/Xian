using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	internal class StudyRulesServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
            if (serviceType != typeof(IStudyRulesService))
                return null;

		    return new StudyRulesServiceProxy();
		}

		#endregion
	}

    internal class StudyRulesServiceProxy : IStudyRulesService
    {
        public StudyRulesServiceProxy()
        {
            Real = new StudyRulesService();
        }

        private IStudyRulesService Real { get; set; }

        public GetRulesResponse GetRules(GetRulesRequest request)
        {
            return ServiceProxyHelper.Call(Real.GetRules, request);
        }

        public GetRuleResponse GetRule(GetRuleRequest request)
        {
            return ServiceProxyHelper.Call(Real.GetRule, request);
        }

        public PutRuleResponse PutRule(PutRuleRequest request)
        {
            return ServiceProxyHelper.Call(Real.PutRule, request);
        }

        public DeleteRuleResponse DeleteRule(DeleteRuleRequest request)
        {
            return ServiceProxyHelper.Call(Real.DeleteRule, request);
        }
    }

    internal class StudyRulesService : IStudyRulesService
	{
		#region Implementation of IStudyRulesService

		public GetRulesResponse GetRules(GetRulesRequest request)
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetRuleBroker();
				var rules = broker.GetRules();
				return new GetRulesResponse(rules.Select(r => r.RuleData).ToList());
			}
		}

		public GetRuleResponse GetRule(GetRuleRequest request)
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetRuleBroker();
				var rule = broker.GetRule(request.RuleId);
				return new GetRuleResponse(rule.RuleData);
			}
		}

		public PutRuleResponse PutRule(PutRuleRequest request)
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetRuleBroker();
				var rule = broker.GetRule(request.Rule.Id);
				if(rule == null)
				{
					rule = new Rule
							{
								RuleId = request.Rule.Id,
								Name = request.Rule.Name,
							};

					broker.AddRule(rule);
				}

				rule.RuleData = request.Rule;
				context.Commit();

				return new PutRuleResponse();
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
