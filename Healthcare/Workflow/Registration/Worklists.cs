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
        public abstract class RegistrationWorklist : WorklistBase
        {
            public abstract string WorklistClassName { get; }

            public override IList GetWorklist(IPersistenceContext context)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                IList<WorklistItem> worklist = broker.GetWorklist(this.WorklistClassName);
                return (IList)worklist;
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Scheduled : RegistrationWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class CheckIn : RegistrationWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : RegistrationWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Completed : RegistrationWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Completed"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Cancelled : RegistrationWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Cancelled"; }
            }
        }
    }
}
