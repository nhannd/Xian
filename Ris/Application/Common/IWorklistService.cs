#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Common.Serialization;

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
