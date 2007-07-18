using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class ToBeReported : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetToBeReportedWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetToBeReportedWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetInProgressWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetInProgressWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InTranscription : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetInTranscriptionWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetInTranscriptionWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class ToBeVerified : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetToBeVerifiedWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetToBeVerifiedWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Verified : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetVerifiedWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetVerifiedWorklistCount(currentUserStaff);
            }
        }

    }
}
