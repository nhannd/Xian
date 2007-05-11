using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Scheduled : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetScheduledWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                return GetBroker(context).GetScheduledWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class CheckedIn : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetCheckedInWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                return GetBroker(context).GetCheckedInWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetInProgressWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                return GetBroker(context).GetInProgressWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Suspended : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetSuspendedWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                return GetBroker(context).GetSuspendedWorklistCount();
            }
       }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Cancelled : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetCancelledWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                return GetBroker(context).GetCancelledWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Completed : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetCompletedWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                return GetBroker(context).GetCompletedWorklistCount();
            }
        }
    }
}
