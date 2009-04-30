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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ReassignComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ReassignComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ReassignComponent class
	/// </summary>
	[AssociateView(typeof(ReassignComponentViewExtensionPoint))]
	public class ReassignComponent : ApplicationComponent
	{
		private readonly ReportingWorklistItem _worklistItem;
		private StaffSummary _radiologist;
		private ILookupHandler _radiologistLookupHandler;

		public ReassignComponent(ReportingWorklistItem item)
		{
			_worklistItem = item;
		}

		public override void Start()
		{
			// create supervisor lookup handler, using filters supplied in application settings
			string filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
			string[] staffTypes = string.IsNullOrEmpty(filters)
				? new string[] { }
				: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); }).ToArray();
			_radiologistLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

			base.Start();
		}

		#region Presentation Model

		[ValidateNotNull]
		public StaffSummary Radiologist
		{
			get { return _radiologist; }
			set
			{
				if (!Equals(value, _radiologist))
				{
					_radiologist = value;
					NotifyPropertyChanged("Radiologist");
				}
			}
		}

		public ILookupHandler RadiologistLookupHandler
		{
			get { return _radiologistLookupHandler; }
		}

		public void Accept()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
						{
							service.ReassignProcedureStep(new ReassignProcedureStepRequest(
								_worklistItem.ProcedureStepRef, 
								_radiologist.StaffRef));
						});
				
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
				this.Exit(ApplicationComponentExitCode.Error);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion


	}
}
