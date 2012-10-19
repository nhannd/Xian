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
			public const string PatientProfileSearch = "PatientProfile:Search";
		}

		internal abstract class SearchOperationRecorderBase : RisServiceOperationRecorderBase
		{
			[DataContract]
			protected class SearchOperationData : OperationData
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
		}

		internal class SearchWorklists : SearchOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (WorklistItemTextQueryRequest) recorderContext.Request;
				return request.UseAdvancedSearch ?
					new SearchOperationData(Operations.WorklistSearch, request.SearchFields)
					: new SearchOperationData(Operations.WorklistSearch, request.TextQuery);
			}
		}

		internal class SearchPatientProfiles : SearchOperationRecorderBase
		{

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				return new SearchOperationData(Operations.PatientProfileSearch, ((TextQueryRequest)recorderContext.Request).TextQuery);
			}
		}
	}
}
