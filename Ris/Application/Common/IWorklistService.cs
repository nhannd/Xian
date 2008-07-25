using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [ServiceContract]
	public interface IWorklistService<TItemSummary>
		where TItemSummary : DataContractBase
    {

		/// <summary>
		/// Obtain the list of worklists for the current user.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ListWorklistsForUserResponse ListWorklistsForUser(ListWorklistsForUserRequest request);

		/// <summary>
		/// Queries a specified worklist.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
        [OperationContract]
        QueryWorklistResponse<TItemSummary> QueryWorklist(QueryWorklistRequest request);

		/// <summary>
		/// Searches worklists based on specified text query.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		TextQueryResponse<TItemSummary> SearchWorklists(WorklistItemTextQueryRequest request);
	}
}
