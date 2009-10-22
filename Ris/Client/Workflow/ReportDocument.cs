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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ReportDocument : Document
	{
		private readonly ReportingWorklistItem _worklistItem;
		private readonly bool _shouldOpenImages;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private ReportingComponent _component;

		public ReportDocument(ReportingWorklistItem worklistItem, bool shouldOpenImages, IReportingWorkflowItemToolContext context)
			: base(worklistItem.ProcedureStepRef, context.DesktopWindow)
		{
			_worklistItem = worklistItem;
			_folderName = context.SelectedFolder.Name;
			_shouldOpenImages = shouldOpenImages;

			if (context.SelectedFolder is ReportingWorkflowFolder)
			{
				_worklistRef = ((ReportingWorkflowFolder)context.SelectedFolder).WorklistRef;
				_worklistClassName = ((ReportingWorkflowFolder)context.SelectedFolder).WorklistClassName;
			}
			else
			{
				_worklistRef = null;
				_worklistClassName = null;
			}
		}

		public override string GetTitle()
		{
			return ReportDocument.GetTitle(_worklistItem);
		}

		public override bool SaveAndClose()
		{
			_component.SaveReport(true);
			return base.Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new ReportingComponent(_worklistItem, _folderName, _worklistRef, _worklistClassName, _shouldOpenImages);
			return _component;
		}

		/// <summary>
		/// Indicates if a user interaction cancelled the opening of the <see cref="ReportingComponent"/>
		/// </summary>
		/// <remarks>
		/// Should only be called after <see mref="Open()"/>
		/// </remarks>
		public bool UserCancelled
		{
			get
			{
				Platform.CheckForNullReference(_component, "_component");
				return _component.UserCancelled;
			}
		}

		public static string GetTitle(ReportingWorklistItem item)
		{
			return string.Format("Report - {0} - {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
		}

		public static string StripTitle(string title)
		{
			return title.Replace("Report - ", "");
		}
	}
}
