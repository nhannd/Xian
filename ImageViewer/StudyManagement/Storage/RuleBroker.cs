#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	public class RuleBroker : Broker
	{
		internal RuleBroker(DicomStoreDataContext context) : base(context)
		{
		}

		public IList<RuleCondition> GetRules()
		{
			return (from r in this.Context.RuleConditions
			        select r).ToList();
		}

		public RuleCondition GetRule(int oid)
		{
			return this.Context.RuleConditions.First(r => r.Oid == oid);
		}
	}
}
