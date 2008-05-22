using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Adt;

namespace ClearCanvas.Ris.Client.EmergencyPhysician
{
	public abstract class EmergencyPhysicianWorkflowFolder : WorkflowFolder<RegistrationWorklistItem>
	{
		private readonly RegistrationWorkflowFolderSystemBase _folderSystem;
		private readonly EntityRef _worklistRef;

		public EmergencyPhysicianWorkflowFolder(RegistrationWorkflowFolderSystemBase folderSystem, string folderName, string folderDescription, EntityRef worklistRef)
			: base(folderSystem, folderName, folderDescription, new RegistrationWorklistTable())
		{
			_folderSystem = folderSystem;

			this.ResourceResolver = new ResourceResolver(this.GetType().Assembly, this.ResourceResolver);

			_worklistRef = worklistRef;
		}

		public EmergencyPhysicianWorkflowFolder(RegistrationWorkflowFolderSystemBase folderSystem, string folderName)
			: this(folderSystem, folderName, null, null)
		{
		}

		protected override bool CanQuery()
		{
			return true;
		}

		protected override QueryItemsResult QueryItems()
		{
			QueryItemsResult result = null;
			Platform.GetService<IRegistrationWorkflowService>(
				delegate(IRegistrationWorkflowService service)
					{
						QueryWorklistRequest request = _worklistRef == null
						                               	? new QueryWorklistRequest(this.WorklistClassName, true, true)
						                               	: new QueryWorklistRequest(_worklistRef, true, true);

						QueryWorklistResponse<RegistrationWorklistItem> response = service.QueryWorklist(request);
						result = new QueryItemsResult(response.WorklistItems, response.ItemCount);
					});

			return result;
		}

		protected override int QueryCount()
		{
			int count = -1;
			Platform.GetService<IRegistrationWorkflowService>(
				delegate(IRegistrationWorkflowService service)
					{
						QueryWorklistRequest request = _worklistRef == null
						                               	? new QueryWorklistRequest(this.WorklistClassName, false, true)
						                               	: new QueryWorklistRequest(_worklistRef, false, true);

						QueryWorklistResponse<RegistrationWorklistItem> response = service.QueryWorklist(request);
						count = response.ItemCount;
					});

			return count;
		}

		public bool GetOperationEnablement(string operationName)
		{
			return _folderSystem.GetOperationEnablement(operationName);
		}
	}
}