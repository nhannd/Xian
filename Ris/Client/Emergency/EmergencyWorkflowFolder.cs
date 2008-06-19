using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Adt;

namespace ClearCanvas.Ris.Client.Emergency
{
	public abstract class EmergencyWorkflowFolder : WorklistFolder<RegistrationWorklistItem, IRegistrationWorkflowService>
	{
		public EmergencyWorkflowFolder()
			: base(new RegistrationWorklistTable())
		{
		}
	}
}