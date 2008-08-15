using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.WorkQueueAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IWorkQueueAdminService))]
	public class WorkQueueAdminService : ApplicationServiceBase, IWorkQueueAdminService
	{
		#region IWorkQueueAdminService Members

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.System.WorkQueue)]
		public GetWorkQueueFormDataResponse GetWorkQueueFormData(GetWorkQueueFormDataRequest request)
		{
			List<EnumValueInfo> statuses = EnumUtils.GetEnumValueList<WorkQueueStatusEnum>(this.PersistenceContext);

			List<string> types = new List<string>(PersistenceContext.GetBroker<IWorkQueueItemBroker>().GetTypes());
			return new GetWorkQueueFormDataResponse(statuses, types);
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.System.WorkQueue)]
		public ListWorkQueueItemsResponse ListWorkQueueItems(ListWorkQueueItemsRequest request)
		{
			WorkQueueItemSearchCriteria criteria = new WorkQueueItemSearchCriteria();
			criteria.CreationTime.SortAsc(0);

			if (request.Status != null)
			{
				criteria.Status.EqualTo(EnumUtils.GetEnumValue<WorkQueueStatus>(request.Status));
			}

			if (request.Type != null)
			{
				criteria.Type.EqualTo(request.Type);
			}

			if (!string.IsNullOrEmpty(request.User))
			{
				criteria.User.StartsWith(request.User);
			}

			if (request.StartTime.HasValue && request.EndTime.HasValue)
			{
				criteria.CreationTime.Between(request.StartTime.Value, request.EndTime.Value);
			}
			else if(request.StartTime.HasValue)
			{
				criteria.CreationTime.MoreThanOrEqualTo(request.StartTime.Value);
			}
			else if (request.StartTime.HasValue)
			{
				criteria.CreationTime.LessThanOrEqualTo(request.EndTime.Value);
			}

			WorkQueueAssembler assembler = new WorkQueueAssembler();
			return new ListWorkQueueItemsResponse(
				CollectionUtils.Map<WorkQueueItem, WorkQueueItemSummary>(
					this.PersistenceContext.GetBroker<IWorkQueueItemBroker>().Find(criteria, request.Page),
					delegate(WorkQueueItem item) { return assembler.CreateWorkQueueItemSummary(item, this.PersistenceContext); }));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.System.WorkQueue)]
		public PurgeCompletedWorkQueueItemsResponse PurgeCompletedWorkQueueItems(PurgeCompletedWorkQueueItemsRequest request)
		{
			throw new NotImplementedException();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.System.WorkQueue)]
		public ResubmitWorkQueueItemResponse ResubmitWorkQueueItem(ResubmitWorkQueueItemRequest request)
		{
			WorkQueueItem item = this.PersistenceContext.Load<WorkQueueItem>(request.WorkQueueItemRef);
			item.Resubmit();
			this.PersistenceContext.SynchState();
			return new ResubmitWorkQueueItemResponse(new WorkQueueAssembler().CreateWorkQueueItemSummary(item, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.System.WorkQueue)]
		public RemoveWorkQueueItemResponse RemoveWorkQueueItem(RemoveWorkQueueItemRequest request)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
