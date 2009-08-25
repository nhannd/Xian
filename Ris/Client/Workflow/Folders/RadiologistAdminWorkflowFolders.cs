#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public abstract class RadiologistAdmin
	{
		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminUnreportedWorklist)]
		[FolderPath("Unreported Items", true)]
		public class ReportingAdminUnreportedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminAssignedWorklist)]
		[FolderPath("Active Reporting Items", true)]
		public class ReportingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ProtocollingAdminAssignedWorklist)]
		[FolderPath("Active Protocolling Items", true)]
		public class ProtocollingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class RadiologistAdminSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItem, IReportingWorkflowService>
		{
			public RadiologistAdminSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			protected override string ProcedureStepClassName
			{
				//TODO: having the client specify the class name isn't a terribly good idea, but
				//it is the only way to get things working right now
				//This class uses two different ProcedureStepClassNames for query.  So this property is actually not used.
				get { return "ReportingProcedureStep and ProtocolAssignmentStep"; }
			}

			protected override TextQueryResponse<ReportingWorklistItem> DoQuery(WorklistSearchParams query, int specificityThreshold)
			{
				TextQueryResponse<ReportingWorklistItem> response;

				WorklistItemTextQueryOptions options = WorklistItemTextQueryOptions.ProcedureStepStaff
					| (DowntimeRecovery.InDowntimeRecoveryMode ? WorklistItemTextQueryOptions.DowntimeRecovery : 0);

				response = DoQueryCore(query, specificityThreshold, options, "ReportingProcedureStep");
				if (response.TooManyMatches)
					return response;

				List<ReportingWorklistItem> storeMatches = new List<ReportingWorklistItem>(response.Matches);
				response = DoQueryCore(query, specificityThreshold, options, "ProtocolAssignmentStep");

				if (!response.TooManyMatches)
					response.Matches.AddRange(storeMatches);

				return response;
			}
		}
	}
}
