using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    [ExtensionPoint]
    public class ModalityWorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(ModalityWorklistExtensionPoint))]
        public class Scheduled : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetScheduledWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetScheduledWorklistCount();
            }
        }

        [ExtensionOf(typeof(ModalityWorklistExtensionPoint))]
        public class CheckedIn : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetCheckedInWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetCheckedInWorklistCount();
            }
        }

        [ExtensionOf(typeof(ModalityWorklistExtensionPoint))]
        public class InProgress : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetInProgressWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetInProgressWorklistCount();
            }
        }

        [ExtensionOf(typeof(ModalityWorklistExtensionPoint))]
        public class Suspended : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetSuspendedWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetSuspendedWorklistCount();
            }
       }

        [ExtensionOf(typeof(ModalityWorklistExtensionPoint))]
        public class Cancelled : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetCancelledWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetCancelledWorklistCount();
            }
        }

        [ExtensionOf(typeof(ModalityWorklistExtensionPoint))]
        public class Completed : WorklistBase<IModalityWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetCompletedWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetCompletedWorklistCount();
            }
        }
    }
}
