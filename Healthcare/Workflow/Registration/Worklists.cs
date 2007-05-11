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

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Scheduled : WorklistBase<IRegistrationWorklistBroker>
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
        public class CheckIn : WorklistBase<IRegistrationWorklistBroker>
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetCheckInWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                return GetBroker(context).GetCheckInWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : WorklistBase<IRegistrationWorklistBroker>
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
        public class Completed : WorklistBase<IRegistrationWorklistBroker>
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

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Cancelled : WorklistBase<IRegistrationWorklistBroker>
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
    }
}
