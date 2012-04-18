#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
    public static class RulesNamespace
    {
        public const string Value = StudyManagementNamespace.Value + "/rules";
    }
    
    #region Request/Response classes

    [DataContract(Namespace = RulesNamespace.Value)]
	public class GetRulesRequest : DataContractBase
	{
	}

    [DataContract(Namespace = RulesNamespace.Value)]
	public class GetRulesResponse : DataContractBase
	{
		public GetRulesResponse(List<RuleData> rules)
		{
			Rules = rules;
		}

		[DataMember]
		public List<RuleData> Rules { get; set; }
	}

    [DataContract(Namespace = RulesNamespace.Value)]
	public class GetRuleRequest : DataContractBase
	{
		public GetRuleRequest(string ruleId)
		{
			RuleId = ruleId;
		}

		[DataMember]
		public string RuleId { get; set; }
	}

    [DataContract(Namespace = RulesNamespace.Value)]
	public class GetRuleResponse : DataContractBase
	{
		public GetRuleResponse(RuleData rule)
		{
			Rule = rule;
		}

		[DataMember]
		public RuleData Rule { get; set; }
	}

    [DataContract(Namespace = RulesNamespace.Value)]
	public class PutRuleRequest : DataContractBase
	{
		public PutRuleRequest(RuleData rule)
		{
			Rule = rule;
		}

		[DataMember]
		public RuleData Rule { get; set; }
	}

    [DataContract(Namespace = RulesNamespace.Value)]
	public class PutRuleResponse : DataContractBase
	{
	}

    [DataContract(Namespace = RulesNamespace.Value)]
	public class DeleteRuleRequest : DataContractBase
	{
		public DeleteRuleRequest(string ruleId)
		{
			RuleId = ruleId;
		}

		[DataMember]
		public string RuleId { get; set; }
	}

    [DataContract(Namespace = RulesNamespace.Value)]
	public class DeleteRuleResponse : DataContractBase
	{
	}

	#endregion

    [ServiceContract(ConfigurationName = "IStudyRulesService", Namespace = RulesNamespace.Value)]
    public interface IStudyRulesService
	{
        [OperationContract]
        GetRulesResponse GetRules(GetRulesRequest request);
        [OperationContract]
        GetRuleResponse GetRule(GetRuleRequest request);
        [OperationContract]
        PutRuleResponse PutRule(PutRuleRequest request);
        [OperationContract]
        DeleteRuleResponse DeleteRule(DeleteRuleRequest request);
	}
}
