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
        public abstract class ModalityWorklist : WorklistBase
        {
            public abstract string WorklistClassName { get; }

            public override IList GetWorklist(IPersistenceContext context)
            {
                IModalityWorklistBroker broker = context.GetBroker<IModalityWorklistBroker>();
                IList<WorklistItem> worklist = broker.GetWorklist(this.WorklistClassName);
                return (IList)worklist;
            }

            public override int GetWorklistCount(IPersistenceContext context)
            {
                IModalityWorklistBroker broker = context.GetBroker<IModalityWorklistBroker>();
                return broker.GetWorklistCount(this.WorklistClassName);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Scheduled : ModalityWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class CheckedIn : ModalityWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Modality.Worklists+CheckedIn"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : ModalityWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Modality.Worklists+InProgress"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Suspended : ModalityWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Suspended"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Cancelled : ModalityWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Cancelled"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Completed : ModalityWorklist
        {
            public override string WorklistClassName
            {
                get { return "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Completed"; }
            }
        }

    }
}
