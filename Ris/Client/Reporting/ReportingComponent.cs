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
using System.Threading;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
	/// Defines an interface for providing a custom report editor.
	/// </summary>
	public interface IReportEditorProvider
	{
		IReportEditor GetEditor(IReportingContext context);
	}

	/// <summary>
	/// Defines an interface for providing a custom report editor page with access to the reporting
	/// context.
	/// </summary>
	public interface IReportingContext
	{
		/// <summary>
		/// Gets the reporting worklist item.
		/// </summary>
		ReportingWorklistItem WorklistItem { get; }

		/// <summary>
		/// Gets the report associated with the worklist item.  Modifications made to this object
		/// will not be persisted.  Use the <see cref="ReportContent"/> property to update the report content.
		/// </summary>
		ReportDetail Report { get; }

		/// <summary>
		/// Gets the index of the active report part (the part that is being edited).
		/// </summary>
		int ActiveReportPartIndex { get; }

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
		#region IReportingContext implementation

		class ReportingContext : IReportingContext
		{
			private readonly ReportingComponent _owner;

			public ReportingContext(ReportingComponent owner)
			{
				_owner = owner;
			}

			public ReportingWorklistItem WorklistItem
			{
				get { return _owner._worklistItem; }
			}

			public ReportDetail Report
			{
				get { return _owner._report; }
			}

			public int ActiveReportPartIndex
			{
				get { return _owner._activeReportPartIndex; }
			}

			public bool CanVerify
			{
				get { return _owner.CanVerify; }
			}

			public bool CanSendToBeVerified
			{
				get { return _owner.CanSendToBeVerified; }
			}

			public bool CanSendToTranscription
			{
				get { return _owner.CanSendToTranscription; }
			}

            public string ReportContent
            {
                get { return _owner.ReportContent; }
                set { _owner.ReportContent = value; }
            }
    
            public Dictionary<string, string> ExtendedProperties
			{
				get { return _owner._reportPartExtendedProperties; }
				set { _owner._reportPartExtendedProperties = value; }
			}

			public StaffSummary Supervisor
			{
				get { return _owner._supervisor; }
				set
				{
					_owner.SetSupervisor(value);
				}
			}

			public void RequestClose(ReportEditorCloseReason reason)
			{
				_owner.RequestClose(reason);
			}
		}

		#endregion

		private readonly ReportingWorklistItem _worklistItem;

		private BannerComponent _bannerComponent;
		private ChildComponentHost _bannerHost;

		private IReportEditor _reportEditor;
		private ChildComponentHost _reportEditorHost;

		private PriorReportComponent _priorReportComponent;
		private ChildComponentHost _priorReportHost;

		private OrderDetailViewComponent _orderDetailComponent;
		private ChildComponentHost _orderDetailHost;

		private OrderAdditionalInfoComponent _orderAdditionalInfoComponent;
		private ChildComponentHost _orderAdditionalInfoHost;

		private bool _canCompleteInterpretationAndVerify;
		private bool _canCompleteVerification;
		private bool _canCompleteInterpretationForVerification;
		private bool _canCompleteInterpretationForTranscription;

		private ReportDetail _report;
		private int _activeReportPartIndex;
		private ILookupHandler _supervisorLookupHandler;
		private StaffSummary _supervisor;
		private Dictionary<string, string> _reportPartExtendedProperties;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReportingComponent(ReportingWorklistItem worklistItem)
		{
			_worklistItem = worklistItem;
		}

		public override void Start()
		{
			_bannerComponent = new BannerComponent(_worklistItem);
			_bannerHost = new ChildComponentHost(this.Host, _bannerComponent);
			_bannerHost.StartComponent();

			_priorReportComponent = new PriorReportComponent(_worklistItem);
			_priorReportHost = new ChildComponentHost(this.Host, _priorReportComponent);
			_priorReportHost.StartComponent();

			_orderDetailComponent = new OrderDetailViewComponent(_worklistItem);
			_orderDetailHost = new ChildComponentHost(this.Host, _orderDetailComponent);
			_orderDetailHost.StartComponent();

			_orderAdditionalInfoComponent = new OrderAdditionalInfoComponent();
			_orderAdditionalInfoHost = new ChildComponentHost(this.Host, _orderAdditionalInfoComponent);
			_orderAdditionalInfoHost.StartComponent();

			_supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, new string[] { "PRAD" });

			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					GetOperationEnablementResponse enablementResponse = service.GetOperationEnablement(new GetOperationEnablementRequest(_worklistItem.ProcedureStepRef));
					_canCompleteInterpretationAndVerify = enablementResponse.OperationEnablementDictionary["CompleteInterpretationAndVerify"];
					_canCompleteVerification = enablementResponse.OperationEnablementDictionary["CompleteVerification"];
					_canCompleteInterpretationForVerification = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForVerification"];
					_canCompleteInterpretationForTranscription = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForTranscription"];

					LoadReportForEditResponse response = service.LoadReportForEdit(new LoadReportForEditRequest(_worklistItem.ProcedureStepRef));
					_report = response.Report;
					_activeReportPartIndex = response.ReportPartIndex;
					_orderAdditionalInfoComponent.OrderExtendedProperties = response.OrderExtendedProperties;

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
							GetRadiologistListResponse getRadListresponse = service.GetRadiologistList(new GetRadiologistListRequest(SupervisorSettings.Default.SupervisorID));
							_supervisor = CollectionUtils.FirstElement(getRadListresponse.Radiologists);
						}
					}
				});

			// check for a report editor provider.  If not found, use the default one
			IReportEditorProvider provider = CollectionUtils.FirstElement<IReportEditorProvider>(
													new ReportEditorProviderExtensionPoint().CreateExtensions());

			_reportEditor = provider == null ? new ReportEditorComponent(new ReportingContext(this)) : provider.GetEditor(new ReportingContext(this));
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

		#region Presentation Model

		public ApplicationComponentHost BannerHost
		{
			get { return _bannerHost; }
		}

		public ApplicationComponentHost ReportEditorHost
		{
			get { return _reportEditorHost; }
		}

		public ApplicationComponentHost PriorReportsHost
		{
			get { return _priorReportHost; }
		}

		public ApplicationComponentHost OrderDetailsHost
		{
			get { return _orderDetailHost; }
		}

		public ApplicationComponentHost OrderAdditionalInfoHost
		{
			get { return _orderAdditionalInfoHost; }
		}

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
        
        public bool VerifyEnabled
		{
			get { return CanVerify; }
		}

		public bool SendToVerifyEnabled
		{
			get { return CanSendToBeVerified; }
		}

		public bool SendToTranscriptionEnabled
		{
			get { return CanSendToTranscription; }
		}

		public bool SendToTranscriptionVisible
		{
			get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UseTranscriptionWorkflow); }
		}

		public bool VerifyReportVisible
		{
			get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport); }
		}

		public bool SupervisorVisible
		{
			get { return !Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UnsupervisedReporting); }
		}

		public void Verify()
		{
			if (!_reportEditor.Save(ReportEditorCloseReason.Verify))
				return;
			try
			{
				if (_canCompleteInterpretationAndVerify)
				{
					Platform.GetService<IReportingWorkflowService>(
						delegate(IReportingWorkflowService service)
						{
							service.CompleteInterpretationAndVerify(
								new CompleteInterpretationAndVerifyRequest(
								_worklistItem.ProcedureStepRef,
								_reportPartExtendedProperties,
								_supervisor == null ? null : _supervisor.StaffRef));
						});
				}
				else if (_canCompleteVerification)
				{
					Platform.GetService<IReportingWorkflowService>(
						delegate(IReportingWorkflowService service)
						{
							service.CompleteVerification(new CompleteVerificationRequest(_worklistItem.ProcedureStepRef, _reportPartExtendedProperties));
						});
				}

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
				this.Exit(ApplicationComponentExitCode.Accepted);
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

		public void SendToBeVerified()
		{
			if (!_reportEditor.Save(ReportEditorCloseReason.SendToBeVerified))
				return;
			try
			{
				if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport) == false && _supervisor == null)
				{
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageChooseRadiologist, MessageBoxActions.Ok);
					return;
				}

				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						service.CompleteInterpretationForVerification(
							new CompleteInterpretationForVerificationRequest(
							_worklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef));
					});

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
				this.Exit(ApplicationComponentExitCode.Accepted);
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

		public void SendToTranscription()
		{
			if (!_reportEditor.Save(ReportEditorCloseReason.SendToTranscription))
				return;
			try
			{
				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						service.CompleteInterpretationForTranscription(
							new CompleteInterpretationForTranscriptionRequest(
							_worklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef));
					});

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.InTranscriptionFolder));
				this.Exit(ApplicationComponentExitCode.Accepted);
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

		public void SaveReport()
		{
			if (!_reportEditor.Save(ReportEditorCloseReason.SaveDraft))
				return;
			try
			{
				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						service.SaveReport(
							new SaveReportRequest(
							_worklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef));
					});

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
				this.Exit(ApplicationComponentExitCode.Accepted);
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

		public void CancelEditing()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		#region Private Helpers

		private bool CanVerify
		{
			get { return (_canCompleteInterpretationAndVerify || _canCompleteVerification) && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport); }
		}

		private bool CanSendToBeVerified
		{
			get { return _canCompleteInterpretationForVerification; }
		}

		private bool CanSendToTranscription
		{
			get { return _canCompleteInterpretationForTranscription && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UseTranscriptionWorkflow); }
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

		#endregion
	}
}
