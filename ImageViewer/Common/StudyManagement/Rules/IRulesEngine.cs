﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
    public class RulesEngineExtensionPoint : ExtensionPoint<IRulesEngine>    
    {}

    public interface IRulesEngine
    {
        bool ApplyAutoRouteActions { get; set; }
        bool ApplyDeleteActions { get; set; }

        /// <summary>
        /// Apply the Study level rules to a Study.
        /// </summary>
        /// <param name="study">The study to apply the rules to.</param>
        /// <param name="workItem">The WorkItem applying the rules.</param>
        void ApplyStudyRules(StudyEntry study, WorkItemData workItem);

        /// <summary>
        /// Apply the Study level rules to a Study.
        /// </summary>
        /// <param name="study">The study to apply the rules to.</param>
        void ApplyStudyRules(StudyEntry study);

        /// <summary>
        /// Apply the a Rule to all matching studies.
        /// </summary>
        /// <param name="rule">The rule to apply.</param>
        /// <param name="study">The study to apply the rule to</param>
        void ApplyStudyToRule(StudyEntry study, RuleData rule);
    }
}