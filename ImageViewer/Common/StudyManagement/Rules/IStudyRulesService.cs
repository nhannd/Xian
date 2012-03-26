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
	public class ListRulesRequest : DataContractBase
	{
	}

	[DataContract]
	public class ListRulesResponse : DataContractBase
	{
		public ListRulesResponse(List<RuleData> rules)
		{
			Rules = rules;
		}

		[DataMember]
		public List<RuleData> Rules { get; set; }
	}

	[DataContract]
	public class AddRuleRequest : DataContractBase
	{
		[DataMember]
		public RuleData Rule { get; set; }
	}

	[DataContract]
	public class AddRuleResponse : DataContractBase
	{
		public AddRuleResponse(RuleData rule)
		{
			Rule = rule;
		}

		[DataMember]
		public RuleData Rule { get; set; }
	}

	[DataContract]
	public class UpdateRuleRequest : DataContractBase
	{
		[DataMember]
		public RuleData Rule { get; set; }
	}

	[DataContract]
	public class UpdateRuleResponse : DataContractBase
	{
		public UpdateRuleResponse(RuleData rule)
		{
			Rule = rule;
		}

		[DataMember]
		public RuleData Rule { get; set; }
	}

	[DataContract]
	public class DeleteRuleRequest : DataContractBase
	{
		public DeleteRuleRequest(int ruleId)
		{
			RuleId = ruleId;
		}

		[DataMember]
		public int RuleId { get; set; }
	}

	[DataContract]
	public class DeleteRuleResponse : DataContractBase
	{
	}

	#endregion

	public interface IStudyRulesService
	{
		ListRulesResponse ListRules(ListRulesRequest request);
		AddRuleResponse AddRule(AddRuleRequest request);
		UpdateRuleResponse UpdateRule(UpdateRuleRequest request);
		DeleteRuleResponse DeleteRule(DeleteRuleRequest request);
	}
}
