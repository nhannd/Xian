#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Security.Permissions;
using System.Threading;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines an interface for providing custom pages to be displayed in the reporting component.
	/// </summary>
	public interface IReportingPageProvider : IExtensionPageProvider<IReportingPage, IReportingContext>
	{
	}

	/// <summary>
	/// Defines an interface to a custom reporting page.
	/// </summary>
	public interface IReportingPage : IExtensionPage
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom page with access to the reporting context.
	/// </summary>
	public interface IReportingContext
	{
		/// <summary>
		/// Gets the reporting worklist item.
		/// </summary>
		ReportingWorklistItem WorklistItem { get; }

		/// <summary>
		/// Occurs to indicate that the <see cref="WorklistItem"/> property has changed,
		/// meaning the entire reporting context is now focused on a different report.
		/// </summary>
		event EventHandler WorklistItemChanged;

		/// <summary>
		/// Gets the report associated with the worklist item.  Modifications made to this object
		/// will not be persisted.
		/// </summary>
		ReportDetail Report { get; }

		/// <summary>
		/// Gets the index of the active report part (the part that is being edited).
		/// </summary>
		int ActiveReportPartIndex { get; }

		/// <summary>
		/// Gets the order detail associated with the report.
		/// </summary>
		OrderDetail Order { get; }
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the reporting component.
	/// </summary>
	[ExtensionPoint]
	public class ReportingPageProviderExtensionPoint : ExtensionPoint<IReportingPageProvider>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom report editor.
	/// </summary>
	public interface IReportEditorProvider
	{
		IReportEditor GetEditor(IReportEditorContext context);
	}

	/// <summary>
	/// Defines an interface for providing a custom report editor page with access to the reporting
	/// context.
	/// </summary>
	public interface IReportEditorContext : IReportingContext
	{
		/// <summary>
		/// Gets a value indicating whether the Verify operation is enabled.
		/// </summary>
		bool CanVerify { get; }

		/// <summary>
		/// Gets a value indicating whether the Send To Verify operation is enabled.
		/// </summary>
		bool CanSendToBeVerified { get; }

		/// <summary>
		/// Gets a value indicating whether the Send To Transcription operation is enabled.
		/// </summary>
		bool CanSendToTranscription { get; }

		/// <summary>
		/// Gets a value indicating whether the Save operation is enabled.
		/// </summary>
		bool CanSaveReport { get; }

		/// <summary>
		/// Gets or sets the report content for the active report part.
		/// </summary>
		string ReportContent { get; set; }

        /// <summary>
        /// Gets or sets the extended properties for the active report part.
        /// </summary>
        Dictionary<string, string> ExtendedProperties { get; set; }
        
        /// <summary>
		/// Gets or sets the supervisor for the active report part.
		/// </summary>
		StaffSummary Supervisor { get; set; }

		/// <summary>
		/// Allows the report editor to request that the reporting component close.
		/// For example, the report editor may wish to have a microphone button initiate the 'Verify' action.
		/// </summary>
		/// <param name="reason"></param>
		void RequestClose(ReportEditorCloseReason reason);
	}

	/// <summary>
	/// Defines possible reasons that the report editor is closing.
	/// </summary>
	public enum ReportEditorCloseReason
	{
		/// <summary>
		/// User has cancelled editing, leaving the report in its current state.
		/// </summary>
		CancelEditing,

		/// <summary>
		/// Report is saved in its current state.
		/// </summary>
		SaveDraft,

		/// <summary>
		/// Report is saved for transcription.
		/// </summary>
		SendToTranscription,

		/// <summary>
		/// Report is saved to be verified later.
		/// </summary>
		SendToBeVerified,

		/// <summary>
		/// Report is saved and verified immediately.
		/// </summary>
		Verify
	}

	/// <summary>
	/// Defines an interface to a custom report editor.
	/// </summary>
	public interface IReportEditor
	{
		/// <summary>
		/// Gets the report editor application component.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetComponent();

		/// <summary>
		/// Instructs the report editor to save the report content and any other data.
		/// The report editor may be veto the action by returning false.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns>True to continue with save, or false to veto the operation.</returns>
		bool Save(ReportEditorCloseReason reason);
	}

	/// <summary>
	/// Defines an extension point for providing a custom report editor.
	/// </summary>
	[ExtensionPoint]
	public class ReportEditorProviderExtensionPoint : ExtensionPoint<IReportEditorProvider>
	{
	}





	/// <summary>
	/// Extension point for views onto <see cref="ReportingComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ReportingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ReportingComponent class
	/// </summary>
	[AssociateView(typeof(ReportingComponentViewExtensionPoint))]
	public class ReportingComponent : ApplicationComponent
	{

		#region ReportingContext

		class ReportingContext : IReportingContext
		{
			private readonly ReportingComponent _owner;

			public ReportingContext(ReportingComponent owner)
			{
				_owner = owner;
			}

			public ReportingWorklistItem WorklistItem
			{
				get { return _owner.WorklistItem; }
			}

			public event EventHandler WorklistItemChanged
			{
				add { _owner._worklistItemManager.WorklistItemChanged += value; }
				remove { _owner._worklistItemManager.WorklistItemChanged -= value; }
			}

			public ReportDetail Report
			{
				get { return _owner._report; }
			}

			public int ActiveReportPartIndex
			{
				get { return _owner._activeReportPartIndex; }
			}

			public OrderDetail Order
			{
				get { return _owner._orderDetail; }
			}

			protected ReportingComponent Owner
			{
				get { return _owner; }
			}
		}

		#endregion

		#region ReportEditorContext

		class ReportEditorContext : ReportingContext, IReportEditorContext
		{
			public ReportEditorContext(ReportingComponent owner)
				:base(owner)
			{
			}

			public bool CanVerify
			{
				get { return Owner.CanVerify; }
			}

			public bool CanSendToBeVerified
			{
				get { return Owner.CanSendToBeVerified; }
			}

			public bool CanSendToTranscription
			{
				get { return Owner.CanSendToTranscription; }
			}

			public bool CanSaveReport
			{
				get { return Owner.SaveReportEnabled; }
			}

            public string ReportContent
            {
				get { return Owner.ReportContent; }
				set { Owner.ReportContent = value; }
            }
    
            public Dictionary<string, string> ExtendedProperties
			{
				get { return Owner._reportPartExtendedProperties; }
				set { Owner._reportPartExtendedProperties = value; }
			}

			public StaffSummary Supervisor
			{
				get { return Owner._supervisor; }
				set
				{
					Owner.SetSupervisor(value);
				}
			}

			public void RequestClose(ReportEditorCloseReason reason)
			{
				Owner.RequestClose(reason);
			}
		}

		#endregion

		private readonly ReportingComponentWorklistItemManager _worklistItemManager;

		private IReportEditor _reportEditor;
		private ChildComponentHost _reportEditorHost;
		private ChildComponentHost _bannerHost;

		private ChildComponentHost _rightHandComponentContainerHost;
		private TabComponentContainer _rightHandComponentContainer;

		private bool _canCompleteInterpretationAndVerify;
		private bool _canCompleteVerification;
		private bool _canCompleteInterpretationForVerification;
		private bool _canCompleteInterpretationForTranscription;
		private bool _canSaveReport;

		private ReportDetail _report;
		private OrderDetail _orderDetail;
		private int _activeReportPartIndex;
		private ILookupHandler _supervisorLookupHandler;
		private StaffSummary _supervisor;
		private Dictionary<string, string> _reportPartExtendedProperties;

		private PriorReportComponent _priorReportComponent;
		private ReportingOrderDetailViewComponent _orderComponent;
		private OrderAdditionalInfoSummaryComponent _additionalInfoComponent;

		private List<IReportingPage> _extensionPages;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReportingComponent(ReportingWorklistItem worklistItem, string folderName, EntityRef worklistRef, string worklistClassName)
		{
			_worklistItemManager = new ReportingComponentWorklistItemManager(worklistItem, folderName, worklistRef, worklistClassName);
			_worklistItemManager.WorklistItemChanged += OnWorklistItemChangedEvent;
		}

		#region ApplicationComponent overrides

		public override void Start()
		{
			StartReportingWorklistItem();

			_bannerHost = new ChildComponentHost(this.Host, new BannerComponent(this.WorklistItem));
			_bannerHost.StartComponent();

			_rightHandComponentContainer = new TabComponentContainer();
			_rightHandComponentContainer.ValidationStrategy = new AllComponentsValidationStrategy();

			_orderComponent = new ReportingOrderDetailViewComponent(this.WorklistItem.OrderRef);
			_rightHandComponentContainer.Pages.Add(new TabPage("Order", _orderComponent));

			_priorReportComponent = new PriorReportComponent(this.WorklistItem);
			_rightHandComponentContainer.Pages.Add(new TabPage("Priors", _priorReportComponent));

			_additionalInfoComponent = new OrderAdditionalInfoSummaryComponent();
			_additionalInfoComponent.OrderExtendedProperties = _orderDetail.ExtendedProperties;
			_rightHandComponentContainer.Pages.Add(new TabPage("Additional Info", _additionalInfoComponent));

			// instantiate all extension pages
			_extensionPages = new List<IReportingPage>();
			foreach (IReportingPageProvider pageProvider in new ReportingPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new ReportingContext(this)));
			}

			// add extension pages to container and set initial context
			// the container will start those components if the user goes to that page
			foreach (IReportingPage page in _extensionPages)
			{
				_rightHandComponentContainer.Pages.Add(new TabPage(page.Path.LocalizedPath, page.GetComponent()));
			}

			_rightHandComponentContainerHost = new ChildComponentHost(this.Host, _rightHandComponentContainer);
			_rightHandComponentContainerHost.StartComponent();

			// create supervisor lookup handler, using filters supplied in application settings
			string filters = ReportingSettings.Default.SupervisorLookupStaffTypeFilters;
			string[] staffTypes = string.IsNullOrEmpty(filters) 
				? new string[] { } 
				: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); }).ToArray();
			_supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

			// check for a report editor provider.  If not found, use the default one
			IReportEditorProvider provider = CollectionUtils.FirstElement<IReportEditorProvider>(
													new ReportEditorProviderExtensionPoint().CreateExtensions());

			_reportEditor = provider == null ? new ReportEditorComponent(new ReportEditorContext(this)) : provider.GetEditor(new ReportEditorContext(this));
			_reportEditorHost = new ChildComponentHost(this.Host, _reportEditor.GetComponent());
			_reportEditorHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_reportEditor != null)
			{
                _reportEditorHost.StopComponent();

                if (_reportEditor is IDisposable)
                {
                    ((IDisposable)_reportEditor).Dispose();
                    _reportEditor = null;
                }
			}

			base.Stop();
		}

		public override bool HasValidationErrors
		{
			get
			{
				return _reportEditorHost.Component.HasValidationErrors || base.HasValidationErrors;
			}
		}

		public override void ShowValidation(bool show)
		{
			_reportEditorHost.Component.ShowValidation(show);
			base.ShowValidation(show);
		}

		#endregion

		private ReportingWorklistItem WorklistItem
		{
			get { return _worklistItemManager.WorklistItem; }
		}

		#region Presentation Model

		public ApplicationComponentHost BannerHost
		{
			get { return _bannerHost; }
		}

		public ApplicationComponentHost ReportEditorHost
		{
			get { return _reportEditorHost; }
		}

		public ApplicationComponentHost RightHandComponentContainerHost
		{
			get { return _rightHandComponentContainerHost; }
		}

		public string StatusText
		{
			get { return _worklistItemManager.StatusText; }
		}

		public bool StatusTextVisible
		{
			get { return _worklistItemManager.StatusTextVisible; }
		}

		public bool ReportNextItem
		{
			get { return _worklistItemManager.ReportNextItem; }
			set { _worklistItemManager.ReportNextItem = value; }
		}

		public bool ReportNextItemEnabled
		{
			get { return _worklistItemManager.ReportNextItemEnabled; }
		}

		public string ReportContent
		{
			get
			{
				if (_reportPartExtendedProperties == null || !_reportPartExtendedProperties.ContainsKey(ReportPartDetail.ReportContentKey))
					return null;

				return _reportPartExtendedProperties[ReportPartDetail.ReportContentKey];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					if (_reportPartExtendedProperties != null && _reportPartExtendedProperties.ContainsKey(ReportPartDetail.ReportContentKey))
					{
						_reportPartExtendedProperties.Remove(ReportPartDetail.ReportContentKey);
					}
				}
				else
				{
					if (_reportPartExtendedProperties == null)
						_reportPartExtendedProperties = new Dictionary<string, string>();

					_reportPartExtendedProperties[ReportPartDetail.ReportContentKey] = value;
				}
			}
		}

		#region Supervisor

		public StaffSummary Supervisor
		{
			get { return _supervisor; }
			set
			{
				if (!Equals(value, _supervisor))
				{
					_supervisor = value;
					NotifyPropertyChanged("Supervisor");
				}
			}
		}

		public ILookupHandler SupervisorLookupHandler
		{
			get { return _supervisorLookupHandler; }
		}

		#endregion

		#region Verify

		public void Verify()
		{
			try
			{
				if(this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				if (!_reportEditor.Save(ReportEditorCloseReason.Verify))
					return;

                // check for a prelim diagnosis
                if (PreliminaryDiagnosis.ConversationExists(this.WorklistItem.OrderRef))
                {
                    if (PreliminaryDiagnosis.ShowConversationDialog(this.WorklistItem.OrderRef, this.Host.DesktopWindow)
                        == ApplicationComponentExitCode.None)
                        return;   // user cancelled out
                }

				if (_canCompleteInterpretationAndVerify)
				{
					Platform.GetService<IReportingWorkflowService>(
						delegate(IReportingWorkflowService service)
						{
							service.CompleteInterpretationAndVerify(
								new CompleteInterpretationAndVerifyRequest(
								this.WorklistItem.ProcedureStepRef,
								_reportPartExtendedProperties,
								_supervisor == null ? null : _supervisor.StaffRef));
						});
				}
				else if (_canCompleteVerification)
				{
					Platform.GetService<IReportingWorkflowService>(
						delegate(IReportingWorkflowService service)
						{
							service.CompleteVerification(new CompleteVerificationRequest(this.WorklistItem.ProcedureStepRef, _reportPartExtendedProperties));
						});
				}

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeVerifiedFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.VerifiedFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
					delegate
					{
						this.Exit(ApplicationComponentExitCode.Error);
					});
			}
		}

		public bool VerifyEnabled
		{
			get { return CanVerify; }
		}

		public bool VerifyReportVisible
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify); }
		}

		#endregion

		#region Send To Be Verified

		public void SendToBeVerified()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				if (!_reportEditor.Save(ReportEditorCloseReason.SendToBeVerified))
					return;

				bool supervisorRequired = !Thread.CurrentPrincipal.IsInRole(
					ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.OmitSupervisor);
				supervisorRequired &= _supervisor == null;

				if (supervisorRequired)
				{
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageChooseRadiologist, MessageBoxActions.Ok);
					return;
				}

				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						service.CompleteInterpretationForVerification(
							new CompleteInterpretationForVerificationRequest(
								_worklistItemManager.WorklistItem.ProcedureStepRef,
								_reportPartExtendedProperties,
								_supervisor == null ? null : _supervisor.StaffRef));
					});

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.AwaitingReviewFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
					delegate
					{
						this.Exit(ApplicationComponentExitCode.Error);
					});
			}
		}

		public bool SendToVerifyEnabled
		{
			get { return CanSendToBeVerified; }
		}

		#endregion

		#region Send To Transcription

		public void SendToTranscription()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				if (!_reportEditor.Save(ReportEditorCloseReason.SendToTranscription))
					return;

				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						service.CompleteInterpretationForTranscription(
							new CompleteInterpretationForTranscriptionRequest(
							this.WorklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef));
					});

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.InTranscriptionFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
					delegate
					{
						this.Exit(ApplicationComponentExitCode.Error);
					});
			}
		}

		public bool SendToTranscriptionEnabled
		{
			get { return CanSendToTranscription; }
		}

		public bool SendToTranscriptionVisible
		{
			get { return ReportingSettings.Default.EnableTranscriptionWorkflow; }
		}

		#endregion

		#region Save

		public void SaveReport()
		{
			try
			{
				if (!_reportEditor.Save(ReportEditorCloseReason.SaveDraft))
					return;

				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						service.SaveReport(
							new SaveReportRequest(
							this.WorklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef));
					});

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.VerifiedFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeVerifiedFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToSaveReport, this.Host.DesktopWindow,
					delegate
					{
						this.Exit(ApplicationComponentExitCode.Error);
					});
			}
		}

		public bool SaveReportEnabled
		{
			get { return CanSaveReport; }
		}

		#endregion

		#region Skip

		public void Skip()
		{
			try
			{
				if (_worklistItemManager.ShouldUnclaim)
				{
					Platform.GetService<IReportingWorkflowService>(
						delegate(IReportingWorkflowService service)
						{
							service.CancelReportingStep(new CancelReportingStepRequest(this.WorklistItem.ProcedureStepRef));
						});
				}
				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Skipped);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool SkipEnabled
		{
			get { return _worklistItemManager.CanSkipItem; }
		}

		#endregion

		#region Cancel

		public void CancelEditing()
		{
			try
			{
				if (_worklistItemManager.ShouldUnclaim)
				{
					Platform.GetService<IReportingWorkflowService>(
						delegate(IReportingWorkflowService service)
						{
							service.CancelReportingStep(new CancelReportingStepRequest(this.WorklistItem.ProcedureStepRef));
						});
				}

				this.Exit(ApplicationComponentExitCode.None);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		#endregion

		#region Private Helpers

		private bool CanVerify
		{
			get
			{
				 return (_canCompleteInterpretationAndVerify || _canCompleteVerification)
					 && Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify);
			}
		}

		private bool CanSendToBeVerified
		{
			get
			{
				 return _canCompleteInterpretationForVerification;
			}
		}

		private bool CanSendToTranscription
		{
			get
			{
				 return _canCompleteInterpretationForTranscription
					 && ReportingSettings.Default.EnableTranscriptionWorkflow;
			}
		}

		private bool CanSaveReport
		{
			get { return _canSaveReport; }
		}


		private void SetSupervisor(StaffSummary supervisor)
		{
			_supervisor = supervisor;
			SupervisorSettings.Default.SupervisorID = supervisor == null ? "" : supervisor.StaffId;
			SupervisorSettings.Default.Save();
		}

		private void RequestClose(ReportEditorCloseReason reason)
		{
			switch (reason)
			{
				case ReportEditorCloseReason.SaveDraft:
					SaveReport();
					break;
				case ReportEditorCloseReason.SendToTranscription:
					SendToTranscription();
					break;
				case ReportEditorCloseReason.SendToBeVerified:
					SendToBeVerified();
					break;
				case ReportEditorCloseReason.Verify:
					Verify();
					break;
				case ReportEditorCloseReason.CancelEditing:
					CancelEditing();
					break;
			}
		}

		private void OnWorklistItemChangedEvent(object sender, EventArgs args)
		{
			if (this.WorklistItem != null)
			{
				StartReportingWorklistItem();
				UpdateChildComponents();
				OpenImages();
			}
			else
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		private void StartReportingWorklistItem()
		{
			bool result = ClaimAndLinkWorklistItem(this.WorklistItem);

			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					GetOperationEnablementResponse enablementResponse = service.GetOperationEnablement(new GetOperationEnablementRequest(this.WorklistItem));
					_canCompleteInterpretationAndVerify = enablementResponse.OperationEnablementDictionary["CompleteInterpretationAndVerify"];
					_canCompleteVerification = enablementResponse.OperationEnablementDictionary["CompleteVerification"];
					_canCompleteInterpretationForVerification = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForVerification"];
					_canCompleteInterpretationForTranscription = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForTranscription"];
					_canSaveReport = enablementResponse.OperationEnablementDictionary["SaveReport"];

					LoadReportForEditResponse response = service.LoadReportForEdit(new LoadReportForEditRequest(this.WorklistItem.ProcedureStepRef));
					_report = response.Report;
					_activeReportPartIndex = response.ReportPartIndex;
					_orderDetail = response.Order;

					ReportPartDetail activePart = _report.GetPart(_activeReportPartIndex);
					_reportPartExtendedProperties = activePart == null ? null : activePart.ExtendedProperties;
					if (activePart != null && activePart.Supervisor != null)
					{
						// active part already has a supervisor assigned
						_supervisor = activePart.Supervisor;
					}
					else
					{
						// active part does not have a supervisor assigned
						// if this user has a default supervisor, retreive it, otherwise leave supervisor as null
						if (!String.IsNullOrEmpty(SupervisorSettings.Default.SupervisorID))
						{
							object supervisor;
							if(_supervisorLookupHandler.Resolve(SupervisorSettings.Default.SupervisorID, false, out supervisor))
							{
								_supervisor = (StaffSummary) supervisor;
							}
						}
					}
				});
		}

		private void UpdateChildComponents()
		{
			((BannerComponent)_bannerHost.Component).HealthcareContext = this.WorklistItem;
			_priorReportComponent.WorklistItem = this.WorklistItem;
			_orderComponent.Context = new OrderDetailViewComponent.OrderContext(this.WorklistItem.OrderRef);
			_additionalInfoComponent.OrderExtendedProperties = _orderDetail.ExtendedProperties;

			this.Host.Title = ReportDocument.GetTitle(this.WorklistItem);

			NotifyPropertyChanged("StatusText");
		}

		private void OpenImages()
		{
			if (ViewImagesHelper.IsSupported)
				ViewImagesHelper.OpenStudy(this.WorklistItem.AccessionNumber);
		}

		#endregion


		private bool ClaimAndLinkWorklistItem(ReportingWorklistItem item)
		{
			if (item.ProcedureStepName == StepType.Interpretation && item.ActivityStatus.Code == StepState.Scheduled)
			{
				// if creating a new report, check for linked interpretations

				List<ReportingWorklistItem> linkedInterpretations;
				bool ok = PromptForLinkedInterpretations(item, out linkedInterpretations);
				if (!ok)
					return false;

				// start the interpretation step
				// note: updating only the ProcedureStepRef is hacky - the service should return an updated item
				item.ProcedureStepRef = StartInterpretation(item, linkedInterpretations);
			}
			else if (item.ProcedureStepName == StepType.Verification && item.ActivityStatus.Code == StepState.Scheduled)
			{
				// start the verification step
				// note: updating only the ProcedureStepRef is hacky - the service should return an updated item
				if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify))
					item.ProcedureStepRef = StartVerification(item);
			}
			return true;
		}


		private bool PromptForLinkedInterpretations(ReportingWorklistItem item, out List<ReportingWorklistItem> linkedItems)
		{
			linkedItems = new List<ReportingWorklistItem>();

			// query server for link candidates
			List<ReportingWorklistItem> candidates = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					GetLinkableInterpretationsRequest request = new GetLinkableInterpretationsRequest(item.ProcedureStepRef);
					candidates = service.GetLinkableInterpretations(request).IntepretationItems;
				});

			// if there are candidates, prompt user to select
			if (candidates.Count > 0)
			{
				LinkedInterpretationComponent component = new LinkedInterpretationComponent(candidates);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					this.Host.DesktopWindow, component, SR.TitleLinkProcedures);
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					linkedItems.AddRange(component.SelectedItems);
					return true;
				}
				return false;
			}
			else
			{
				// no candidates
				return true;
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify)]
		private EntityRef StartVerification(ReportingWorklistItem item)
		{
			EntityRef result = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					StartVerificationResponse response = service.StartVerification(new StartVerificationRequest(item.ProcedureStepRef));
					result = response.VerificationStepRef;
				});

			return result;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
		private EntityRef StartInterpretation(ReportingWorklistItem item, List<ReportingWorklistItem> linkedInterpretations)
		{
			List<EntityRef> linkedInterpretationRefs = linkedInterpretations.ConvertAll<EntityRef>(
				delegate(ReportingWorklistItem x) { return x.ProcedureStepRef; });

			EntityRef result = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					StartInterpretationRequest request = new StartInterpretationRequest(item.ProcedureStepRef, linkedInterpretationRefs);
					StartInterpretationResponse response = service.StartInterpretation(request);
					result = response.InterpretationStepRef;
				});

			return result;
		}
	}
}
