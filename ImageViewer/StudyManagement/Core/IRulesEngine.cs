#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
	public class RulesEngineExtensionPoint : ExtensionPoint<IRulesEngine>
	{ }

	public interface IRulesEngine
	{
		/// <summary>
		/// Apply the Study level rules to a Study.
		/// </summary>
		/// <param name="study">The study to apply the rules to.</param>
		/// <param name="options"> </param>
		void ApplyStudyRules(StudyEntry study, RuleApplicationOptions options);

		/// <summary>
		/// Apply the specified rule to the specified study.
		/// </summary>
		/// <param name="study">The study to apply the rule to</param>
		/// <param name="ruleId"> The rule to apply.</param>
		/// <param name="options"> </param>
		void ApplyStudyRule(StudyEntry study, string ruleId, RuleApplicationOptions options);
	}
}
