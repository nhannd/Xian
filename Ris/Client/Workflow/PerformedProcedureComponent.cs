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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to capture information about a performed procedure step.
	/// </summary>
	public interface IPerformedStepEditorPageProvider : IExtensionPageProvider<IPerformedStepEditorPage, IPerformedStepEditorContext>
	{
		IPerformedStepEditorPage[] GetEditorPages(IPerformedStepEditorContext context);
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IPerformedStepEditorContext
	{
		/// <summary>
		/// Gets a reference to the order.
		/// </summary>
		EntityRef OrderRef { get; }

		/// <summary>
		/// Gets a reference to the patient.
		/// </summary>
		EntityRef PatientRef { get; }

		/// <summary>
		/// Gets a reference to the patient profile.
		/// </summary>
		EntityRef PatientProfileRef { get; }

		/// <summary>
		/// Gets the currently selected performed procedure step.
		/// </summary>
		ModalityPerformedProcedureStepDetail SelectedPerformedStep { get; }

		/// <summary>
		/// Occurs when the <see cref="SelectedPerformedStep"/> property changes.
		/// </summary>
		event EventHandler SelectedPerformedStepChanged;

		/// <summary>
		/// Exposes the extended properties associated with the <see cref="SelectedPerformedStep"/>.  Modifications made to these
		/// properties by the editor page will be persisted whenever the editor is saved.
		/// </summary>
		IDictionary<string, string> SelectedPerformedStepExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the performed procedure step editor.
	/// </summary>
	[ExtensionPoint]
	public class PerformedStepEditorPageProviderExtensionPoint : ExtensionPoint<IPerformedStepEditorPageProvider>
	{
	}

	/// <summary>
	/// Defines an interface to a custom performed procedure step editor page.
	/// </summary>
	public interface IPerformedStepEditorPage : IExtensionPage
	{
		void Save();
	}


	/// <summary>
	/// Extension point for views onto <see cref="PerformedProcedureComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PerformedProcedureComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PerformedProcedureComponent class
	/// </summary>
	[AssociateView(typeof(PerformedProcedureComponentViewExtensionPoint))]
	public class PerformedProcedureComponent : ApplicationComponent
	{
		#region EditorContext

		class EditorContext : IPerformedStepEditorContext
		{
			private readonly PerformedProcedureComponent _owner;

			public EditorContext(PerformedProcedureComponent owner)
			{
				_owner = owner;
			}

			public EntityRef OrderRef
			{
				get { return _owner._worklistItem.OrderRef; }
			}

			public EntityRef PatientRef
			{
				get { return _owner._worklistItem.PatientRef; }
			}

			public EntityRef PatientProfileRef
			{
				get { return _owner._worklistItem.PatientProfileRef; }
			}

			public event EventHandler SelectedPerformedStepChanged
			{
				add { _owner._selectedMppsChanged += value; }
				remove { _owner._selectedMppsChanged -= value; }
			}

			public ModalityPerformedProcedureStepDetail SelectedPerformedStep
			{
				get { return _owner._selectedMpps; }
			}

			public IDictionary<string, string> SelectedPerformedStepExtendedProperties
			{
				get { return _owner._selectedMpps == null ? null : _owner._selectedMpps.ExtendedProperties; }
			}
		}

		#endregion

		#region MPPS Details Component

		class MppsDetailsComponent : DHtmlComponent, IPerformedStepEditorPage
		{
			#region HealthcareContext

			[DataContract]
			class HealthcareContext : DataContractBase
			{
				private readonly MppsDetailsComponent _owner;

				public HealthcareContext(MppsDetailsComponent owner)
				{
					_owner = owner;
				}

				[DataMember]
				public EntityRef OrderRef
				{
					get { return _owner._context.OrderRef; }
				}

				[DataMember]
				public EntityRef PatientRef
				{
					get { return _owner._context.PatientRef; }
				}

				[DataMember]
				public EntityRef PatientProfileRef
				{
					get { return _owner._context.PatientProfileRef; }
				}

				[DataMember]
				public ModalityPerformedProcedureStepDetail ModalityPerformedProcedureStep
				{
					get { return _owner._context.SelectedPerformedStep; }
				}
			}

			#endregion

			private readonly IPerformedStepEditorContext _context;

			public MppsDetailsComponent(IPerformedStepEditorContext context)
			{
				_context = context;
			}

			public override void Start()
			{
				// when the selected step changes, refresh the browser
				_context.SelectedPerformedStepChanged += delegate
				{
					SetUrl(WebResourcesSettings.Default.DetailsPageUrl);
				};

				base.Start();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return new HealthcareContext(this);
			}

			protected override IDictionary<string, string> TagData
			{
				get { return _context.SelectedPerformedStepExtendedProperties; }
			}

			#region IPerformedStepEditorPage Members

			Path IExtensionPage.Path
			{
				get { return new Path("Details", new ResourceResolver(this.GetType().Assembly)); }
			}

			IApplicationComponent IExtensionPage.GetComponent()
			{
				return this;
			}

			void IPerformedStepEditorPage.Save()
			{
				SaveData();
			}

			#endregion
		}

		#endregion

		private EntityRef _orderRef;
		private WorklistItemSummaryBase _worklistItem;

		private readonly PerformingDocumentationMppsSummaryTable _mppsTable = new PerformingDocumentationMppsSummaryTable();
		private ModalityPerformedProcedureStepDetail _selectedMpps;
		private event EventHandler _selectedMppsChanged;

		private SimpleActionModel _mppsActionHandler;
		private ClickAction _stopAction;
		private ClickAction _discontinueAction;

		private ChildComponentHost _detailsPagesHost;

		private readonly List<IPerformedStepEditorPage> _editorPages = new List<IPerformedStepEditorPage>();

		private readonly PerformingDocumentationComponent _owner;

		private event EventHandler<ProcedurePlanChangedEventArgs> _procedurePlanChanged;

		/// <summary>
		/// Constructor
		/// </summary>
		public PerformedProcedureComponent(WorklistItemSummaryBase worklistItem, PerformingDocumentationComponent owner)
		{
			Platform.CheckForNullReference(worklistItem, "worklistItem");
			Platform.CheckForNullReference(owner, "owner");

			_worklistItem = worklistItem;
			_orderRef = worklistItem.OrderRef;
			_owner = owner;
		}

		internal void AddPerformedProcedureStep(ModalityPerformedProcedureStepDetail mpps)
		{
			_mppsTable.Items.Add(mpps);
			_mppsTable.Sort();
		}

		internal event EventHandler<ProcedurePlanChangedEventArgs> ProcedurePlanChanged
		{
			add { _procedurePlanChanged += value; }
			remove { _procedurePlanChanged -= value; }
		}

		internal void SaveData()
		{
			foreach (IPerformedStepEditorPage page in _editorPages)
			{
				page.Save();
			}
		}

		internal IList<ModalityPerformedProcedureStepDetail> PerformedProcedureSteps
		{
			get { return _mppsTable.Items; }
		}

		#region ApplicationComponent overrides

		public override void Start()
		{
			ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

			_mppsActionHandler = new SimpleActionModel(resolver);

			_stopAction = _mppsActionHandler.AddAction("stop", SR.TitleStopMpps, "Icons.CheckInToolSmall.png", SR.TitleStopMpps, StopPerformedProcedureStep);
			_discontinueAction = _mppsActionHandler.AddAction("discontinue", SR.TitleDiscontinueMpps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMpps, DiscontinuePerformedProcedureStep);
			UpdateActionEnablement();

			if (_orderRef != null)
			{
				Platform.GetService<IModalityWorkflowService>(
					delegate(IModalityWorkflowService service)
					{
						ListPerformedProcedureStepsRequest mppsRequest = new ListPerformedProcedureStepsRequest(_orderRef);
						ListPerformedProcedureStepsResponse mppsResponse = service.ListPerformedProcedureSteps(mppsRequest);

						_mppsTable.Items.AddRange(mppsResponse.PerformedProcedureSteps);
						_mppsTable.Sort();
					});
			}

			// create extension editor pages, if any exist
			foreach (IPerformedStepEditorPageProvider provider in (new PerformedStepEditorPageProviderExtensionPoint().CreateExtensions()))
			{
				_editorPages.AddRange(provider.GetEditorPages(new EditorContext(this)));
			}

			// if no editor pages are available via extensions, create the default editor
			if (_editorPages.Count == 0)
			{
				_editorPages.Add(new MppsDetailsComponent(new EditorContext(this)));

				if(PerformingDocumentationComponentSettings.Default.ShowDicomSeriesTab)
				{
					_editorPages.Add(new PerformedProcedureDicomSeriesComponent(new EditorContext(this)));
				}
			}


			// if there are multiple pages, need to create a tab container
			if (_editorPages.Count > 1)
			{
				TabComponentContainer tabContainer = new TabComponentContainer();
				_detailsPagesHost = new ChildComponentHost(this.Host, tabContainer);
				foreach (IPerformedStepEditorPage page in _editorPages)
				{
					tabContainer.Pages.Add(new TabPage(page.Path.LocalizedPath, page.GetComponent()));
				}
			}
			else
			{
				// don't create a tab container for just one page
				_detailsPagesHost = new ChildComponentHost(this.Host, _editorPages[0].GetComponent());
			}

			// start details pages host
			_detailsPagesHost.StartComponent();

			base.Start();
		}

        public override void Stop()
        {
            if (_detailsPagesHost != null)
            {
                _detailsPagesHost.StopComponent();
                _detailsPagesHost = null;
            }

            base.Stop();
        }

		public override bool HasValidationErrors
		{
			get
			{
				return _detailsPagesHost.Component.HasValidationErrors || base.HasValidationErrors;
			}
		}

		public override void ShowValidation(bool show)
		{
			_detailsPagesHost.Component.ShowValidation(show);
			base.ShowValidation(show);
		}

		#endregion

		#region Presentation Model

		public ITable ProcedurePlanSummaryTable
		{
			get { return _owner.ProcedurePlanSummaryTable; }
		}

		public ActionModelNode ProcedurePlanTreeActionModel
		{
			get { return _owner.ProcedurePlanTreeActionModel; }
		}

		public ITable MppsTable
		{
			get { return _mppsTable; }
		}

		public ISelection SelectedMpps
		{
			get { return new Selection(_selectedMpps); }
			set
			{
				ModalityPerformedProcedureStepDetail selectedMpps = (ModalityPerformedProcedureStepDetail)value.Item;
				if (selectedMpps != _selectedMpps)
				{
					OnSelectedMppsChanged(selectedMpps);
				}
			}
		}

		public ActionModelNode MppsTableActionModel
		{
			get { return _mppsActionHandler; }
		}

		public ApplicationComponentHost DetailsComponentHost
		{
			get { return _detailsPagesHost; }
		}

		private void RefreshProcedurePlanTree(ProcedurePlanDetail procedurePlanDetail)
		{
			_orderRef = procedurePlanDetail.OrderRef;
			EventsHelper.Fire(_procedurePlanChanged, this, new ProcedurePlanChangedEventArgs(procedurePlanDetail));
		}

		#endregion

		#region Tool Click Handlers

		private void StopPerformedProcedureStep()
		{
			if (_selectedMpps == null)
			{
				return;
			}

			// bail on validation errors
			if (this.HasValidationErrors)
			{
				ShowValidation(true);
				return;
			}

			// if downtime recovery mode, need to get the time from the user
			DateTime? endTime = _selectedMpps.StartTime;
			if(DowntimeRecovery.InDowntimeRecoveryMode)
			{
				if(!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, "Completed Time", false, ref endTime))
					return;
			}

			try
			{
				SaveData();

				CompleteModalityPerformedProcedureStepResponse response = null;
				Platform.GetService<IModalityWorkflowService>(
					delegate(IModalityWorkflowService service)
					{
						CompleteModalityPerformedProcedureStepRequest request = new CompleteModalityPerformedProcedureStepRequest(_selectedMpps);
						request.CompletedTime = DowntimeRecovery.InDowntimeRecoveryMode ? endTime : null;
						response = service.CompleteModalityPerformedProcedureStep(request);
					});

				RefreshProcedurePlanTree(response.ProcedurePlan);

				_mppsTable.Items.Replace(
					delegate(ModalityPerformedProcedureStepDetail mppsSummary)
					{
						return mppsSummary.ModalityPerformendProcedureStepRef.Equals(_selectedMpps.ModalityPerformendProcedureStepRef, true);
					},
					response.StoppedMpps);

				// Refresh selection
				_selectedMpps = response.StoppedMpps;

				UpdateActionEnablement();
				_mppsTable.Sort();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void DiscontinuePerformedProcedureStep()
		{
			try
			{
				ModalityPerformedProcedureStepDetail selectedMpps = _selectedMpps;

				if (selectedMpps != null)
				{
                    if(this.Host.DesktopWindow.ShowMessageBox("Are you sure you want to discontinue selected procedure(s)?", MessageBoxActions.YesNo) != DialogBoxAction.No)
                    {
                        // if downtime recovery mode, need to get the time from the user
                        DateTime? endTime = _selectedMpps.StartTime;
                        if (DowntimeRecovery.InDowntimeRecoveryMode)
                        {
                            if (!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, "Completed Time", false, ref endTime))
                                return;
                        }
                        DiscontinueModalityPerformedProcedureStepResponse response = null;
                        Platform.GetService<IModalityWorkflowService>(
                            delegate(IModalityWorkflowService service)
                                {
                                    DiscontinueModalityPerformedProcedureStepRequest request = new DiscontinueModalityPerformedProcedureStepRequest(selectedMpps);
                                    request.DiscontinuedTime = DowntimeRecovery.InDowntimeRecoveryMode ? endTime : null;
                                    response = service.DiscontinueModalityPerformedProcedureStep(request);
                                });

                        RefreshProcedurePlanTree(response.ProcedurePlan);

                        _mppsTable.Items.Replace(
                            delegate(ModalityPerformedProcedureStepDetail mpps)
                                {
                                    return mpps.ModalityPerformendProcedureStepRef.Equals(_selectedMpps.ModalityPerformendProcedureStepRef, true);
                                }, response.DiscontinuedMpps);

                        _selectedMpps = response.DiscontinuedMpps;
                        UpdateActionEnablement();
                        _mppsTable.Sort();
                    }
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		#region Private Methods

		private void OnSelectedMppsChanged(ModalityPerformedProcedureStepDetail newSelection)
		{
			if (_selectedMpps != null)
			{
				// save changes to existing data first
				SaveData();
			}

			_selectedMpps = newSelection;

			UpdateActionEnablement();

			EventsHelper.Fire(_selectedMppsChanged, this, EventArgs.Empty);
		}

		private void UpdateActionEnablement()
		{
			if (_selectedMpps != null)
			{
				// TOOD:  replace with server side logic
				_stopAction.Enabled = _discontinueAction.Enabled = _selectedMpps.State.Code == "IP";
			}
			else
			{
				_stopAction.Enabled = _discontinueAction.Enabled = false;
			}
		}

		#endregion
	}
}
