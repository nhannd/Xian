using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    [ExtensionPoint]
    public class ReportingWorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class Draft : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetDraftWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetDraftWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
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
