#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.AutoRouteAction
{
    [ExtensionOf(typeof (SampleRuleExtensionPoint))]
    public class MultiTagAutoRoute : SampleRuleBase
    {
        public MultiTagAutoRoute()
            : base("MultiTagAutoRoute",
                   "Multi-Tag AutoRoute",
                   ServerRuleTypeEnum.AutoRoute,
                   "Sample_AutoRouteMultiTag.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
    }

    [ExtensionOf(typeof (SampleRuleExtensionPoint))]
    public class SimpleAutoRouteSample : SampleRuleBase
    {
        public SimpleAutoRouteSample()
            : base("SimpleAutoRoute",
                   "Simple AutoRoute",
                   ServerRuleTypeEnum.AutoRoute,
                   "Sample_AutoRouteSimple.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
    }

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class ScheduleAutoRouteSample : SampleRuleBase
	{
		public ScheduleAutoRouteSample()
			: base("ScheduleAutoRoute",
				   "Schedule AutoRoute",
				   ServerRuleTypeEnum.AutoRoute,
				   "Sample_AutoRouteSchedule.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class SourceAeAutoRouteSample : SampleRuleBase
	{
		public SourceAeAutoRouteSample()
			: base("SourceAeAutoRoute",
				   "AutoRoute based on Source AE Title",
				   ServerRuleTypeEnum.AutoRoute,
				   "Sample_AutoRouteSourceAe.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopEdited);
		}
	}
}