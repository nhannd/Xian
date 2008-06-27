using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Workflow;

namespace ClearCanvas.Ris.Client.Emergency
{
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	public class EmergencyOrdersConversationTool : PreliminaryDiagnosisConversationTool<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
        public override void Initialize()
        {
            base.Initialize();

            this.Context.RegisterDoubleClickHandler(Open);
        }
	}
}
