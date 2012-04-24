using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
    public class RulesActionExtensionPoint : ExtensionPoint<IRulesAction>
    { }

    public interface IRulesAction
    {
        void Apply(StudyEntry study, RuleAction action, WorkItemData workitem);
    }
}
