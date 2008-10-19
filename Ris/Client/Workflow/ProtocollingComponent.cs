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
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ProtocollingComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ProtocollingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProtocollingComponent class
	/// </summary>
	[AssociateView(typeof(ProtocollingComponentViewExtensionPoint))]
	public class ProtocollingComponent : ApplicationComponent
	{
		#region Private Fields

		private readonly ProtocollingComponentMode _componentMode;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private int _completedItems = 0;
		private bool _isInitialItem = true;

		private ReportingWorklistItem _worklistItem;
		private EntityRef _protocolAssignmentStepRef;
		private EntityRef _assignedStaffRef;
		private List<OrderNoteDetail> _notes;

		private readonly List<ReportingWorklistItem> _skippedItems;
		private readonly Stack<ReportingWorklistItem> _worklistCache;

		private ChildComponentHost _bannerComponentHost;
		private ChildComponentHost _protocolEditorComponentHost;
		private ChildComponentHost _orderDetailViewComponentHost;
		private ChildComponentHost _priorReportsComponentHost;
		private ChildComponentHost _orderNotesComponentHost;

		private bool _protocolNextItem;

		private bool _acceptEnabled;
		private bool _submitForApprovalEnabled;
		private bool _rejectEnabled;
		private bool _saveEnabled;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ProtocollingComponent(ReportingWorklistItem worklistItem, ProtocollingComponentMode mode, string folderName, EntityRef worklistRef, string worklistClassName)
		{
			_worklistItem = worklistItem;
			_componentMode = mode;
			_folderName = folderName;
			_worklistRef = worklistRef;
			_worklistClassName = worklistClassName;

			_protocolNextItem = this.CanProtocolMultipleItems;

			_skippedItems = new List<ReportingWorklistItem>();
			_worklistCache = new Stack<ReportingWorklistItem>();
		}

		#endregion

		#region ApplicationComponent overrides

		public override void Start()
		{
			StartProtocollingWorklistItem();

			this.Host.Title = ProtocollingComponentDocument.GetTitle(_worklistItem);

			_bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(_worklistItem));
			_bannerComponentHost.StartComponent();

			_orderNotesComponentHost = new ChildComponentHost(this.Host, new OrderNoteSummaryComponent(OrderNoteCategory.Protocol));
			_orderNotesComponentHost.StartComponent();
			((OrderNoteSummaryComponent) _orderNotesComponentHost.Component).Notes = _notes;

			_protocolEditorComponentHost = new ChildComponentHost(this.Host, new ProtocolEditorComponent(_worklistItem));
			_protocolEditorComponentHost.StartComponent();
			((ProtocolEditorComponent) _protocolEditorComponentHost.Component).CanEdit = this.SaveEnabled;

			_priorReportsComponentHost = new ChildComponentHost(this.Host, new PriorReportComponent(_worklistItem));
			_priorReportsComponentHost.StartComponent();

            _orderDetailViewComponentHost = new ChildComponentHost(this.Host, new ProtocollingOrderDetailViewComponent(_worklistItem.PatientRef, _worklistItem.OrderRef));
			_orderDetailViewComponentHost.StartComponent();

			base.Start();
		}

        public override void Stop()
        {
            if (_bannerComponentHost != null)
            {
                _bannerComponentHost.StopComponent();
                _bannerComponentHost = null;
            }

            if (_orderNotesComponentHost != null)
            {
                _orderNotesComponentHost.StopComponent();
                _orderNotesComponentHost = null;
            }

            if (_protocolEditorComponentHost != null)
            {
                _protocolEditorComponentHost.StopComponent();
                _protocolEditorComponentHost = null;
            }

            if (_priorReportsComponentHost != null)
            {
                _priorReportsComponentHost.StopComponent();
                _priorReportsComponentHost = null;
            }

            if (_orderDetailViewComponentHost != null)
            {
                _orderDetailViewComponentHost.StopComponent();
                _orderDetailViewComponentHost = null;
            }

            base.Stop();
        }

		#endregion

		#region Public members

		public ApplicationComponentHost BannerComponentHost
		{
			get { return _bannerComponentHost; }
		}

		public ApplicationComponentHost ProtocolEditorComponentHost
		{
			get { return _protocolEditorComponentHost; }
		}

		public ApplicationComponentHost OrderNotesComponentHost
		{
			get { return _orderNotesComponentHost; }
		}

		public ApplicationComponentHost OrderDetailViewComponentHost
		{
			get { return _orderDetailViewComponentHost; }
		}

		public ApplicationComponentHost PriorReportsComponentHost
		{
			get { return _priorReportsComponentHost; }
		}

		public string StatusText
		{
			get
			{
				string status = string.Format(SR.FormatProtocolFolderName, _folderName);

				if (!_isInitialItem)
				{
					status = status + string.Format(SR.FormatProtocolStatusText, _worklistCache.Count, _completedItems, _skippedItems.Count);
				}

				return status;
			}
		}

		public bool ShowStatusText
		{
			get { return this.CanProtocolMultipleItems; }
		}

		/// <summary>
		/// Specifies if the next <see cref="ReportingWorklistItem"/> should be protocolled
		/// </summary>
		public bool ProtocolNextItem
		{
			get { return _protocolNextItem; }
			set { _protocolNextItem = value; }
		}

		public bool ProtocolNextItemEnabled
		{
			get { return this.CanProtocolMultipleItems; }
		}

		#region Accept

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Accept)]
		public void Accept()
		{
			// don't allow accept if there are validation errors
			if (HasValidationErrors)
			{
				ShowValidation(true);
				return;
			}

			if (SupervisorRequred())
				return;

			try
			{
				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						service.AcceptProtocol(new AcceptProtocolRequest(_protocolAssignmentStepRef, this.ProtocolDetail, _notes));
					});

				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.CompletedProtocolFolder));
				InvalidateSourceFolders();

				BeginNextWorklistItemOrExit();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool AcceptVisible
		{
			get
			{
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Accept);
			}
		}

		public bool AcceptEnabled
		{
			get { return _acceptEnabled; }
		}

		#endregion

		#region Submit For Approval

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Create)]
		public void SubmitForApproval()
		{
			// don't allow accept if there are validation errors
			if (HasValidationErrors)
			{
				ShowValidation(true);
				return;
			}

			if (SupervisorRequred())
				return;

			try
			{
				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						service.SubmitProtocolForApproval(
							new SubmitProtocolForApprovalRequest(
								_protocolAssignmentStepRef,
								this.ProtocolDetail,
								_notes));
					});

				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.AwaitingApprovalProtocolFolder));
				InvalidateSourceFolders();

				BeginNextWorklistItemOrExit();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool SubmitForApprovalEnabled
		{
			get { return _submitForApprovalEnabled; }
		}

		public bool SubmitForApprovalVisible
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview); }
		}

		#endregion

		#region Reject

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Create)]
		public void Reject()
		{
			if (SupervisorRequred())
				return;

			try
			{
				EnumValueInfo reason;
				string additionalComments;

				bool result = GetSuspendReason("Reject Reason", out reason, out additionalComments);

				if (!result || reason == null)
					return;

				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						service.RejectProtocol(new RejectProtocolRequest(
							_protocolAssignmentStepRef,
							this.ProtocolDetail,
							_notes,
							reason,
							CreateAdditionalCommentsNote(additionalComments)));
					});

				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.RejectedProtocolFolder));
				InvalidateSourceFolders();

				BeginNextWorklistItemOrExit();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool RejectEnabled
		{
			get { return _rejectEnabled; }
		}

		#endregion

		#region Save

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Create)]
		public void Save()
		{
			if (SupervisorRequred())
				return;

			try
			{
				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						service.SaveProtocol(new SaveProtocolRequest(_protocolAssignmentStepRef, this.ProtocolDetail, _notes));
					});

				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeProtocolledFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftProtocolFolder));

				BeginNextWorklistItemOrExit();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool SaveEnabled
		{
			get { return _saveEnabled; }
		}

		#endregion

		#region Skip

		public void Skip()
		{
			try
			{
				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						bool shouldUnclaim = _componentMode == ProtocollingComponentMode.Assign;
						service.DiscardProtocol(new DiscardProtocolRequest(_protocolAssignmentStepRef, _notes, shouldUnclaim, _assignedStaffRef));
					});

				SkipCurrentItemAndBeginNextItemOrExit();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool SkipEnabled
		{
			get { return _protocolNextItem && this.ProtocolNextItemEnabled; }
		}

		#endregion

		#region Close

		public void Close()
		{
			try
			{
				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						bool shouldUnclaim = _componentMode == ProtocollingComponentMode.Assign;
						service.DiscardProtocol(new DiscardProtocolRequest(_protocolAssignmentStepRef, _notes, shouldUnclaim, _assignedStaffRef));
					});

				// To be protocolled folder will be invalid if it is the source of the worklist item;  the original item will have been
				// discontinued with a new scheduled one replacing it
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeProtocolledFolder));

				this.Exit(ApplicationComponentExitCode.None);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		#endregion

		#region Private methods

		private bool CanProtocolMultipleItems
		{
			get { return _componentMode == ProtocollingComponentMode.Assign && (_worklistRef != null || _worklistClassName != null); }
		}

		/// <summary>
		/// Invalidates source folders appropriate to current <see cref="ProtocollingComponentMode"/>
		/// </summary>
		private void InvalidateSourceFolders()
		{
			if (_componentMode == ProtocollingComponentMode.Assign)
			{
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeProtocolledFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeReviewedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.AssignedForReviewProtocolFolder));
			}
			else if (_componentMode == ProtocollingComponentMode.Edit)
			{
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
			}
		}

		private void BeginNextWorklistItemOrExit()
		{
			if (this.ProtocolNextItem)
			{
				_completedItems++;
				LoadNextWorklistItem();
			}
			else
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		private void SkipCurrentItemAndBeginNextItemOrExit()
		{
			// To be protocolled folder will be invalid if it is the source of the worklist item;  the original item will have been
			// discontinued with a new scheduled one replacing it
			DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeProtocolledFolder));

			_skippedItems.Add(_worklistItem);
			LoadNextWorklistItem();
		}

		private void LoadNextWorklistItem()
		{
			try
			{
				_worklistItem = GetNextWorklistItem();

				if (_worklistItem != null)
				{
					_isInitialItem = false;

					StartProtocollingWorklistItem();
					UpdateChildComponents();
				}
				else
				{
					// TODO : Dialog "No more"
					this.Exit(ApplicationComponentExitCode.None);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private ReportingWorklistItem GetNextWorklistItem()
		{
			if (_worklistCache.Count == 0)
			{
				RefreshWorklistItemCache();
			}

			return _worklistCache.Count > 0 ? _worklistCache.Pop() : null;
		}

		private void RefreshWorklistItemCache()
		{
			try
			{
				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						QueryWorklistRequest request = _worklistRef != null
							? new QueryWorklistRequest(_worklistRef, true, true, DowntimeRecovery.InDowntimeRecoveryMode)
							: new QueryWorklistRequest(_worklistClassName, true, true, DowntimeRecovery.InDowntimeRecoveryMode);

						QueryWorklistResponse<ReportingWorklistItem> response = service.QueryWorklist(request);

						foreach (ReportingWorklistItem item in response.WorklistItems)
						{
							if (WorklistItemWasPreviouslySkipped(item) == false)
							{
								_worklistCache.Push(item);
							}
						}
					});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private bool WorklistItemWasPreviouslySkipped(ReportingWorklistItem item)
		{
			return CollectionUtils.Contains(_skippedItems,
				delegate(ReportingWorklistItem skippedItem)
				{
					return skippedItem.AccessionNumber == item.AccessionNumber;
				});
		}

		private void StartProtocollingWorklistItem()
		{
			// begin with validation turned off
			ShowValidation(false);

			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					bool shouldClaim = _componentMode == ProtocollingComponentMode.Assign;

					StartProtocolResponse response = service.StartProtocol(new StartProtocolRequest(_worklistItem.ProcedureStepRef, null, shouldClaim, OrderNoteCategory.Protocol.Key));
					_protocolAssignmentStepRef = response.ProtocolAssignmentStepRef;
					_assignedStaffRef = response.AssignedStaffRef;

					_notes = response.ProtocolNotes;

					if (response.ProtocolClaimed == shouldClaim)
					{
						GetOperationEnablementResponse enablementResponse =
							service.GetOperationEnablement(new GetOperationEnablementRequest(_worklistItem));

						_acceptEnabled = enablementResponse.OperationEnablementDictionary["AcceptProtocol"];
						_rejectEnabled = enablementResponse.OperationEnablementDictionary["RejectProtocol"];
						_submitForApprovalEnabled = enablementResponse.OperationEnablementDictionary["SubmitProtocolForApproval"];
						_saveEnabled = enablementResponse.OperationEnablementDictionary["SaveProtocol"];
					}
					else
					{
						SkipCurrentItemAndBeginNextItemOrExit();
					}
				});
		}

		private void UpdateChildComponents()
		{
			((BannerComponent)_bannerComponentHost.Component).HealthcareContext = _worklistItem;
			((PriorReportComponent)_priorReportsComponentHost.Component).WorklistItem = _worklistItem;
			((ProtocolEditorComponent)_protocolEditorComponentHost.Component).WorklistItem = _worklistItem;
			((ProtocolEditorComponent)_protocolEditorComponentHost.Component).CanEdit = this.SaveEnabled;
			((ProtocollingOrderDetailViewComponent)_orderDetailViewComponentHost.Component).Context = new OrderDetailViewComponent.OrderContext(_worklistItem.OrderRef);

			// Load notes for new current item.
			((OrderNoteSummaryComponent)_orderNotesComponentHost.Component).Notes = _notes;

			// Update title
			this.Host.Title = ProtocollingComponentDocument.GetTitle(_worklistItem);

			NotifyPropertyChanged("StatusText");
		}

		private bool GetSuspendReason(string title, out EnumValueInfo reason, out string additionalComments)
		{
			ProtocolReasonComponent component = new ProtocolReasonComponent();

			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, component, title);

			reason = component.Reason;
			additionalComments = component.OtherReason;

			return exitCode == ApplicationComponentExitCode.Accepted;
		}

		private static OrderNoteDetail CreateAdditionalCommentsNote(string additionalComments)
		{
			if (!string.IsNullOrEmpty(additionalComments))
				return new OrderNoteDetail(OrderNoteCategory.Protocol.Key, additionalComments, null, false, null, null);
			else
				return null;
		}

		private ProtocolDetail ProtocolDetail
		{
			get { return ((ProtocolEditorComponent) _protocolEditorComponentHost.Component).ProtocolDetail; }
		}

		private bool SupervisorRequred()
		{
			bool supervisorRequired =
				!Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.OmitSupervisor)
				&& this.ProtocolDetail.Supervisor != null; 

			if (supervisorRequired)
			{
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageChooseRadiologist, MessageBoxActions.Ok);
			}

			return supervisorRequired;
		}

		#endregion
	}
}
