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
        public class Scheduled : WorklistBase
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)broker.GetScheduledWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return broker.GetScheduledWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class CheckIn : WorklistBase
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)broker.GetCheckInWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return broker.GetCheckInWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : WorklistBase
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)broker.GetInProgressWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return broker.GetInProgressWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Completed : WorklistBase
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)broker.GetCompletedWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return broker.GetCompletedWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Cancelled : WorklistBase
        {
            public override IList GetWorklist(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)broker.GetCancelledWorklist();
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return broker.GetCancelledWorklistCount();
            }
        }
    }
}
