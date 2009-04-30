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

using System;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ProtocollingComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItem, IReportingWorkflowService>
	{
		public ProtocollingComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode<TWorklistITem>(ReportingWorklistItem worklistItem)
		{
			throw new NotSupportedException("Protocolling component mode should be initialized externally.  ReportingWorklistItem does not have enough context.");
		}

		protected override string TaskName
		{
			get { return "Protocolling"; }
		}
	}

	public static class ProtocollingComponentModes
	{
		public static AssignProtocolMode Assign = new AssignProtocolMode();
		public static EditProtocolMode Edit = new EditProtocolMode();
		public static ReviewProtocolMode Review = new ReviewProtocolMode();
	}

	public class AssignProtocolMode : ContinuousWorkflowComponentMode
	{
		public AssignProtocolMode()
			: base(true, true, true)
		{
		}
	}

	public class EditProtocolMode : ContinuousWorkflowComponentMode
	{
		public EditProtocolMode()
			: base(false, false, false)
		{
		}
	}

	public class ReviewProtocolMode : ContinuousWorkflowComponentMode
	{
		public ReviewProtocolMode()
			: base(false, false, false)
		{
		}
	}
}