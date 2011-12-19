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
                   "Grant Access by Referring Physician",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccess.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
		}
    }

    [ExtensionOf(typeof(SampleRuleExtensionPoint))]
    public class GrantAccessSamplesTwoDocs : SampleRuleBase
    {
        public GrantAccessSamplesTwoDocs()
            : base("GrantAccessSamplesTwoDocs",
                   "Grant Access by Two Referring Physicians",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccessTwoDocs.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
        }
    }

    [ExtensionOf(typeof(SampleRuleExtensionPoint))]
    public class GrantAccessSourceAeSamples : SampleRuleBase
    {
        public GrantAccessSourceAeSamples()
            : base("GrantAccessSourceAeSample",
                   "Grant Access by Source AE Title",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccessSourceAe.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
        }
    }

    [ExtensionOf(typeof(SampleRuleExtensionPoint))]
    public class GrantAccessInstitutionSamples : SampleRuleBase
    {
        public GrantAccessInstitutionSamples()
            : base("GrantAccessInstitutionSamples",
                   "Grant Access by Institution Name",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccessInstitution.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
        }
    }
}