#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class WorkflowServiceRecorder
	{
		static class Operations
		{
			public const string WorklistSearch = "Worklist:Search";
		}

		internal class SearchWorklists : RisServiceOperationRecorderBase
		{
			[DataContract]
			class SearchOperationData : OperationData
			{
				public SearchOperationData(string operation, string queryString)
					: base(operation)
				{
					this.SearchString = queryString;
				}

				public SearchOperationData(string operation, object queryParameters)
					: base(operation)
				{
					this.SearchParameters = queryParameters;
				}

				[DataMember]
				public string SearchString;

				[DataMember]
				public object SearchParameters;
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = recorderContext.Request as WorklistItemTextQueryRequest;
				if (request != null && request.UseAdvancedSearch)
				{
					return new SearchOperationData(Operations.WorklistSearch, request.SearchFields);
				}

				return new SearchOperationData(Operations.WorklistSearch, ((TextQueryRequest)recorderContext.Request).TextQuery);
			}
		}
	}
}
