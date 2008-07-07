using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class EmergencyWorkflowFolder : WorklistFolder<RegistrationWorklistItem, IRegistrationWorkflowService>
	{
		public EmergencyWorkflowFolder()
			: base(new RegistrationWorklistTable())
		{
		}
	}
}