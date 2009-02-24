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
