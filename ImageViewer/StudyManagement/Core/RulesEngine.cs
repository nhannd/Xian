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
using System.Text;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
	public static class RulesEngine
	{
		class DefaultEngine : IRulesEngine
		{
			public void ApplyStudyRules(RulesEngineContext context, StudyEntry study)
			{
				throw new NotImplementedException();
			}

			public void ApplyStudyRule(RulesEngineContext context, StudyEntry study, string ruleId)
			{
				throw new NotImplementedException();
			}
		}

		public static IRulesEngine Create()
		{
			var ep = new RulesEngineExtensionPoint();
			return (IRulesEngine)ep.CreateExtensions().FirstOrDefault() ?? new DefaultEngine();
		}
	}
}
