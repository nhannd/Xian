using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	/// <summary>
	/// Provides services for administering the work queue.
	/// </summary>
	[RisServiceProvider]
	[ServiceContract]
	public interface IWorkQueueAdminService
	{
		[OperationContract]
		GetWorkQueueFormDataResponse GetWorkQueueFormData(GetWorkQueueFormDataRequest request);

		/// <summary>
		/// Lists all items in the work queue.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ListWorkQueueItemsResponse ListWorkQueueItems(ListWorkQueueItemsRequest request);

		/// <summary>
		/// Load details for a specified work queue item
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		LoadWorkQueueItemForEditResponse LoadWorkQueueItemForEdit(LoadWorkQueueItemForEditRequest request);

		/// <summary>
		/// Purges all completed work queue items.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		PurgeCompletedWorkQueueItemsResponse PurgeCompletedWorkQueueItems(PurgeCompletedWorkQueueItemsRequest request);

		/// <summary>
		/// Resubmits the specified work queue item for processing.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		ResubmitWorkQueueItemResponse ResubmitWorkQueueItem(ResubmitWorkQueueItemRequest request);

		/// <summary>
		/// Removes the specified work queue item.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		RemoveWorkQueueItemResponse RemoveWorkQueueItem(RemoveWorkQueueItemRequest request);
	}
}
