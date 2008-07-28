using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	public class EmergencyOrdersConversationTool : PreliminaryDiagnosisConversationTool<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
        public override void Initialize()
        {
            base.Initialize();

			this.Context.RegisterDoubleClickHandler(Open, delegate { return this.Enabled; });
        }
	}
}
