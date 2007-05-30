using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Test : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                //return (IList)GetBroker(context).GetWorklist();
                return new List<WorklistItem>();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                //return GetBroker(context).GetScheduledWorklistCount();
                return 0;
            }
        }

    }
}
