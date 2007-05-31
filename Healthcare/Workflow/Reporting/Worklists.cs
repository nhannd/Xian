using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class ScheduledInterpretation : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetScheduledInterpretationWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetScheduledInterpretationWorklistCount();
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class MyInterpretation : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetMyInterpretationWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetMyInterpretationWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class MyTranscription : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetMyTranscriptionWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetMyTranscriptionWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class MyVerification : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetMyVerificationWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetMyVerificationWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class MyVerified : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetMyVerifiedWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetMyVerifiedWorklistCount(currentUserStaff);
            }
        }

    }
}
