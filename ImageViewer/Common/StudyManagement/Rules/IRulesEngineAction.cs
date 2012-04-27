using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
    public class RulesEngineActionExtensionPoint : ExtensionPoint<IRulesEngineAction>
    { }

    public interface IRulesEngineAction
    {
        void Apply(RulesEngineContext rulesEngineContext, StudyEntry study, RuleAction action, WorkItemData workitem);
    }
}
