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
	#region Request/Response classes

	[DataContract]
	public class GetRulesRequest : DataContractBase
	{
	}

	[DataContract]
	public class GetRulesResponse : DataContractBase
	{
		public GetRulesResponse(List<RuleData> rules)
		{
			Rules = rules;
		}

		[DataMember]
		public List<RuleData> Rules { get; set; }
	}

	[DataContract]
	public class GetRuleRequest : DataContractBase
	{
		public GetRuleRequest(string ruleId)
		{
			RuleId = ruleId;
		}

		[DataMember]
		public string RuleId { get; set; }
	}

	[DataContract]
	public class GetRuleResponse : DataContractBase
	{
		public GetRuleResponse(RuleData rule)
		{
			Rule = rule;
		}

		[DataMember]
		public RuleData Rule { get; set; }
	}

	[DataContract]
	public class PutRuleRequest : DataContractBase
	{
		public PutRuleRequest(RuleData rule)
		{
			Rule = rule;
		}

		[DataMember]
		public RuleData Rule { get; set; }
	}

	[DataContract]
	public class PutRuleResponse : DataContractBase
	{
	}

	[DataContract]
	public class DeleteRuleRequest : DataContractBase
	{
		public DeleteRuleRequest(string ruleId)
		{
			RuleId = ruleId;
		}

		[DataMember]
		public string RuleId { get; set; }
	}

	[DataContract]
	public class DeleteRuleResponse : DataContractBase
	{
	}

	#endregion

	public interface IStudyRulesService
	{
		GetRulesResponse GetRules(GetRulesRequest request);
		GetRuleResponse GetRule(GetRuleRequest request);
		PutRuleResponse PutRule(PutRuleRequest request);
		DeleteRuleResponse DeleteRule(DeleteRuleRequest request);
	}
}
