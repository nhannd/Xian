#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	public class RuleBroker : Broker
	{
		internal RuleBroker(DicomStoreDataContext context) : base(context)
		{
		}

		public IList<Rule> GetRules()
		{
			return (from r in this.Context.Rules
					orderby r.Name 
			        select r).ToList();
		}

		public Rule GetRule(string ruleId)
		{
			return this.Context.Rules.FirstOrDefault(r => r.RuleId == ruleId);
		}

		public void AddRule(Rule rule)
		{
			this.Context.Rules.InsertOnSubmit(rule);
		}

		public void DeleteRule(string ruleId)
		{
			var rule = this.Context.Rules.First(r => r.RuleId == ruleId);
			this.Context.Rules.DeleteOnSubmit(rule);
		}
	}
}
