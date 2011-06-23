#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.GrantAccessAction
{
    [ExtensionOf(typeof (SampleRuleExtensionPoint))]
    public class GrantAccessSamples : SampleRuleBase
    {
        public GrantAccessSamples()
            : base("GrantAccessSample",
                   "Simple Grant Access",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccess.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
		}
    }
}