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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public enum WorklistEditorMode
	{
		Add,
		Edit,
		Duplicate
	}

	public class StaffSelectorTable : Table<StaffSummary>
	{
		public StaffSelectorTable()
		{
			this.Columns.Add(new TableColumn<StaffSummary, string>("Name", item => PersonNameFormat.Format(item.Name), 1.0f));
			this.Columns.Add(new TableColumn<StaffSummary, string>("Role", item => item.StaffType.Value, 0.5f));
		}
	}

	/// <summary>
	/// WorklistEditorComponent class
	/// </summary>
	public class WorklistEditorComponent : NavigatorComponentContainer
	{
		class ProcedureTypeGroupTable : Table<ProcedureTypeGroupSummary>
		{
			public ProcedureTypeGroupTable()
			{
				this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnName, summary => summary.Name));
			}
		}

		class LocationTable : Table<LocationSummary>
		{
			public LocationTable()
			{
				this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnName, summary => summary.Name));
			}
		}

		class StaffGroupTable : Table<StaffGroupSummary>
		{
			public StaffGroupTable()
			{
				this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Name", item => item.Name, 1.0f));
			}
		}

		private readonly WorklistEditorMode _mode;
		private readonly bool _adminMode;
		private readonly string _initialClassName;
		private readonly IList<string> _worklistClassChoices;

		private EntityRef _worklistRef;
		private readonly List<WorklistAdminSummary> _editedWorklistSummaries = new List<WorklistAdminSummary>();
		private WorklistAdminDetail _worklistDetail;

		private WorklistDetailEditorComponentBase _detailComponent;
		private WorklistFilterEditorComponent _filterComponent;
		private StaffSelectorEditorComponent _interpretedByFilterComponent;
		private StaffSelectorEditorComponent _transcribedByFilterComponent;
		private StaffSelectorEditorComponent _verifiedByFilterComponent;
		private StaffSelectorEditorComponent _supervisedByFilterComponent;
		private WorklistTimeWindowEditorComponent _timeWindowComponent;
		private SelectorEditorComponent<ProcedureTypeGroupSummary, ProcedureTypeGroupTable> _procedureTypeGroupFilterComponent;
		private SelectorEditorComponent<LocationSummary, LocationTable> _locationFilterComponent;
		private SelectorEditorComponent<StaffSummary, StaffSelectorTable> _staffSubscribersComponent;
		private SelectorEditorComponent<StaffGroupSummary, StaffGroupTable> _groupSubscribersComponent;
		private WorklistSummaryComponent _summaryComponent;
		private NavigatorPage _patientLocationComponentPage;
		private NavigatorPage _interpretedByFilterComponentPage;
		private NavigatorPage _transcribedByFilterComponentPage;
		private NavigatorPage _verifiedByFilterComponentPage;
		private NavigatorPage _supervisedByFilterComponentPage;
		private NavigatorPage _timeWindowComponentPage;
		private NavigatorPage _summaryComponentPage;

		/// <summary>
		/// Constructor to create new worklist(s).
		/// </summary>
		public WorklistEditorComponent(bool adminMode)
			: this(null, adminMode, WorklistEditorMode.Add, null, null)
		{
		}

		/// <summary>
		/// Constructor to create new worklist(s) with limited choices of class.
		/// </summary>
		public WorklistEditorComponent(bool adminMode, IList<string> worklistClassChoices, string selectedClass)
			: this(null, adminMode, WorklistEditorMode.Add, worklistClassChoices, selectedClass)
		{
		}

		/// <summary>
		/// Constructor to edit a worklist.
		/// </summary>
		public WorklistEditorComponent(EntityRef entityRef, bool adminMode)
			: this(entityRef, adminMode, WorklistEditorMode.Edit, null, null)
		{
		}

		/// <summary>
		/// Constructor to duplicate a worklist.
		/// </summary>
		public WorklistEditorComponent(EntityRef entityRef, bool adminMode, IList<string> worklistClassChoices, string selectedClass)
			: this(entityRef, adminMode, WorklistEditorMode.Duplicate, worklistClassChoices, selectedClass)
		{
		}

		/// <summary>
		/// Private constructor.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <param name="adminMode"></param>
		/// <param name="editMode"></param>
		/// <param name="worklistClassChoices"></param>
		/// <param name="selectedClass"></param>
		private WorklistEditorComponent(EntityRef entityRef, bool adminMode, WorklistEditorMode editMode, IList<string> worklistClassChoices, string selectedClass)
		{
			_worklistRef = entityRef;
			_adminMode = adminMode;
			_mode = editMode;
			_worklistClassChoices = worklistClassChoices;
			_initialClassName = selectedClass;

			// start with entire tree expanded
			this.StartFullyExpanded = true;
		}

		public override void Start()
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					var request = new GetWorklistEditFormDataRequest
									{
										GetWorklistEditFormChoicesRequest = new GetWorklistEditFormChoicesRequest(!_adminMode)
									};
					var formDataResponse = service.GetWorklistEditFormData(request).GetWorklistEditFormChoicesResponse;

					// initialize _worklistDetail depending on add vs edit vs duplicate mode
					var procedureTypeGroups = new List<ProcedureTypeGroupSummary>();
					if (_mode == WorklistEditorMode.Add)
					{
						_worklistDetail = new WorklistAdminDetail
							{
								FilterByWorkingFacility = true,
								WorklistClass = CollectionUtils.SelectFirst(formDataResponse.WorklistClasses, wc => wc.ClassName == _initialClassName)
							};

						// establish initial class name
					}
					else
					{
						// load the existing worklist
						var response = service.LoadWorklistForEdit(new LoadWorklistForEditRequest(_worklistRef));

						_worklistDetail = response.Detail;
						_worklistRef = response.Detail.EntityRef;

						// determine initial set of proc type groups, since worklist class already known
						var groupsResponse = service.ListProcedureTypeGroups(new ListProcedureTypeGroupsRequest(_worklistDetail.WorklistClass.ProcedureTypeGroupClassName));
						procedureTypeGroups = groupsResponse.ProcedureTypeGroups;
					}

					// limit class choices if filter specified
					if (_worklistClassChoices != null)
					{
						formDataResponse.WorklistClasses =
							CollectionUtils.Select(formDataResponse.WorklistClasses, wc => _worklistClassChoices.Contains(wc.ClassName));
					}

					// sort worklist classes so they appear alphabetically in editor
					formDataResponse.WorklistClasses = CollectionUtils.Sort(formDataResponse.WorklistClasses, (x, y) => x.DisplayName.CompareTo(y.DisplayName));

					// determine which main page to show (multi or single)
					if (_mode == WorklistEditorMode.Add && _adminMode)
					{
						_detailComponent = new WorklistMultiDetailEditorComponent(formDataResponse.WorklistClasses, formDataResponse.OwnerGroupChoices);
					}
					else
					{
						_detailComponent = new WorklistDetailEditorComponent(
							_worklistDetail,
							formDataResponse.WorklistClasses,
							formDataResponse.OwnerGroupChoices,
							_mode,
							_adminMode,
							false);
					}
					_detailComponent.ProcedureTypeGroupClassChanged += ProcedureTypeGroupClassChangedEventHandler;
					_detailComponent.WorklistClassChanged += OnWorklistClassChanged;

					// create all other pages
					_filterComponent = new WorklistFilterEditorComponent(_worklistDetail,
						procedureTypeGroups, formDataResponse.FacilityChoices, formDataResponse.OrderPriorityChoices,
						formDataResponse.PatientClassChoices);

					_procedureTypeGroupFilterComponent = new SelectorEditorComponent<ProcedureTypeGroupSummary, ProcedureTypeGroupTable>(
						procedureTypeGroups, _worklistDetail.ProcedureTypeGroups, s => s.ProcedureTypeGroupRef);

					_locationFilterComponent = new SelectorEditorComponent<LocationSummary, LocationTable>(
						formDataResponse.PatientLocationChoices, _worklistDetail.PatientLocations, s => s.LocationRef);

					_timeWindowComponent = new WorklistTimeWindowEditorComponent(_worklistDetail);

					_interpretedByFilterComponent = new StaffSelectorEditorComponent(
						formDataResponse.StaffChoices, _worklistDetail.InterpretedByStaff.Staff, _worklistDetail.InterpretedByStaff.IncludeCurrentUser);
					_transcribedByFilterComponent = new StaffSelectorEditorComponent(
						formDataResponse.StaffChoices, _worklistDetail.TranscribedByStaff.Staff, _worklistDetail.TranscribedByStaff.IncludeCurrentUser);
					_verifiedByFilterComponent = new StaffSelectorEditorComponent(
						formDataResponse.StaffChoices, _worklistDetail.VerifiedByStaff.Staff, _worklistDetail.VerifiedByStaff.IncludeCurrentUser);
					_supervisedByFilterComponent = new StaffSelectorEditorComponent(
						formDataResponse.StaffChoices, _worklistDetail.SupervisedByStaff.Staff, _worklistDetail.SupervisedByStaff.IncludeCurrentUser);

					if (ShowSubscriptionPages)
					{
						_staffSubscribersComponent = new SelectorEditorComponent<StaffSummary, StaffSelectorTable>(
							formDataResponse.StaffChoices,
							_worklistDetail.StaffSubscribers,
							s => s.StaffRef,
							SubscriptionPagesReadOnly);
						_groupSubscribersComponent = new SelectorEditorComponent<StaffGroupSummary, StaffGroupTable>(
							formDataResponse.GroupSubscriberChoices,
							_worklistDetail.GroupSubscribers,
							s => s.StaffGroupRef,
							SubscriptionPagesReadOnly);
					}
				});

			// add pages
			this.Pages.Add(new NavigatorPage("NodeWorklist", _detailComponent));
			this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters", _filterComponent));
			this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/NodeProcedureTypeGroups", _procedureTypeGroupFilterComponent));
			this.Pages.Add(_patientLocationComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodePatientLocations", _locationFilterComponent));

			_interpretedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeInterpretedBy", _interpretedByFilterComponent);
			_transcribedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeTranscribedBy", _transcribedByFilterComponent);
			_verifiedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeVerifiedBy", _verifiedByFilterComponent);
			_supervisedByFilterComponentPage = new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeSupervisedBy", _supervisedByFilterComponent);

			_timeWindowComponentPage = new NavigatorPage("NodeWorklist/NodeTimeWindow", _timeWindowComponent);

			ShowWorklistCategoryDependantPages();

			if (ShowSubscriptionPages)
			{
				this.Pages.Add(new NavigatorPage("NodeWorklist/NodeSubscribers/NodeGroupSubscribers", _groupSubscribersComponent));
				this.Pages.Add(new NavigatorPage("NodeWorklist/NodeSubscribers/NodeStaffSubscribers", _staffSubscribersComponent));
			}
			this.Pages.Add(_summaryComponentPage = new NavigatorPage("NodeWorklist/Summary", _summaryComponent = new WorklistSummaryComponent(_worklistDetail, _adminMode)));

			this.CurrentPageChanged += WorklistEditorComponent_CurrentPageChanged;

			this.ValidationStrategy = new AllComponentsValidationStrategy();

			base.Start();

			// Modify EntityRef and add the word "copy" to the worklist name.
			// This is done after the Start() call, so changing the worklist name will trigger a component modify changed.
			if (_mode == WorklistEditorMode.Duplicate)
			{
				_worklistDetail.EntityRef = null;
				((WorklistDetailEditorComponent)_detailComponent).Name = _worklistDetail.Name + " copy";
			}
		}

		private void OnWorklistClassChanged(object sender, EventArgs e)
		{
			ShowWorklistCategoryDependantPages();
		}

		private void ShowWorklistCategoryDependantPages()
		{
			var showStaffFilters = _worklistDetail.WorklistClass == null || ShowReportingStaffRoleFilters;
			ShowAfterPage(_interpretedByFilterComponentPage, showStaffFilters, _patientLocationComponentPage);
			ShowAfterPage(_transcribedByFilterComponentPage, showStaffFilters, _interpretedByFilterComponentPage);
			ShowAfterPage(_verifiedByFilterComponentPage, showStaffFilters, _transcribedByFilterComponentPage);
			ShowAfterPage(_supervisedByFilterComponentPage, showStaffFilters, _verifiedByFilterComponentPage);

			// add the time filter page, if the class supports it (or if the class is not known, in the case of an add)
			var showTimeFilter = _worklistDetail.WorklistClass == null || _worklistDetail.WorklistClass.SupportsTimeWindow;
			ShowBeforePage(_timeWindowComponentPage, showTimeFilter, _summaryComponentPage);
		}

		private void ShowAfterPage(NavigatorPage page, bool show, NavigatorPage insertAfterPage)
		{
			if (show)
			{
				if (this.Pages.Contains(page) == false)
				{
					if (insertAfterPage == null)
						this.Pages.Add(page);
					else
					{
						var index = this.Pages.IndexOf(insertAfterPage);
						this.Pages.Insert(index + 1, page);
					}

				}
			}
			else
			{
				if (this.Pages.Contains(page))
					this.Pages.Remove(page);
			}
		}

		private void ShowBeforePage(NavigatorPage page, bool show, NavigatorPage insertBeforePage)
		{
			if (show)
			{
				if (this.Pages.Contains(page) == false)
				{
					if (insertBeforePage == null)
						this.Pages.Add(page);
					else
					{
						var index = this.Pages.IndexOf(insertBeforePage);
						this.Pages.Insert(index, page);
					}
					
				}
			}
			else
			{
				if (this.Pages.Contains(page))
					this.Pages.Remove(page);
			}
		}

		private void WorklistEditorComponent_CurrentPageChanged(object sender, EventArgs e)
		{
			// Update the summary page when it is active
			if (this.CurrentPage.Component == _summaryComponent)
			{
				UpdateWorklistDetail();

				if (_detailComponent is WorklistMultiDetailEditorComponent)
				{
					var detailEditor = (WorklistMultiDetailEditorComponent)_detailComponent;

					var names = new List<string>();
					var descriptions = new List<string>();
					var classes = new List<WorklistClassSummary>();

					CollectionUtils.ForEach(detailEditor.WorklistsToCreate,
							delegate(WorklistMultiDetailEditorComponent.WorklistTableEntry entry)
								{
									names.Add(entry.Name);
									descriptions.Add(entry.Description);
									classes.Add(entry.Class);
								});

					_summaryComponent.SetMultipleWorklistInfo(names, descriptions, classes);
			}

				_summaryComponent.Refresh();
		}
		}

		private void ProcedureTypeGroupClassChangedEventHandler(object sender, EventArgs e)
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					var groupsResponse = service.ListProcedureTypeGroups(
						new ListProcedureTypeGroupsRequest(_detailComponent.ProcedureTypeGroupClass));

					_procedureTypeGroupFilterComponent.AllItems = groupsResponse.ProcedureTypeGroups;
				});
		}

		public List<WorklistAdminSummary> EditedWorklistSummaries
		{
			get { return _editedWorklistSummaries; }
		}

		#region Presentation Model

		public override void Accept()
		{
			UpdateWorklistDetail();

			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
				try
				{
					if (_mode == WorklistEditorMode.Add || _mode == WorklistEditorMode.Duplicate)
					{
						AddWorklists();
					}
					else
					{
						UpdateWorklist();
					}

					// save any modified user settings
					WorklistEditorComponentSettings.Default.Save();

					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionSaveWorklist, this.Host.DesktopWindow,
						() => this.Exit(ApplicationComponentExitCode.Error));
				}
			}
		}

		public void ItemsAddedOrRemoved()
		{
			this.Modified = true;
		}

		#endregion

		private bool ShowSubscriptionPages
		{
			get { return _adminMode && Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Admin.Data.Worklist); }
		}

		private bool SubscriptionPagesReadOnly
		{
			get { return _worklistDetail.IsUserWorklist; }
		}

		private bool ShowReportingStaffRoleFilters
		{
			get
			{
				return _worklistDetail != null && _worklistDetail.WorklistClass != null
						? _worklistDetail.WorklistClass.SupportsReportingStaffRoleFilters
						: false;
			}
		}

		private void UpdateWorklistDetail()
		{
			if (_filterComponent.IsStarted)
				_filterComponent.SaveData();

			if (_timeWindowComponent.IsStarted)
				_timeWindowComponent.SaveData();

			if (_procedureTypeGroupFilterComponent.IsStarted)
				_worklistDetail.ProcedureTypeGroups = new List<ProcedureTypeGroupSummary>(_procedureTypeGroupFilterComponent.SelectedItems);

			if (_locationFilterComponent.IsStarted)
				_worklistDetail.PatientLocations = new List<LocationSummary>(_locationFilterComponent.SelectedItems);

			if (ShowSubscriptionPages && _groupSubscribersComponent.IsStarted)
				_worklistDetail.GroupSubscribers = new List<StaffGroupSummary>(_groupSubscribersComponent.SelectedItems);

			if (ShowSubscriptionPages && _staffSubscribersComponent.IsStarted)
				_worklistDetail.StaffSubscribers = new List<StaffSummary>(_staffSubscribersComponent.SelectedItems);

			if (ShowReportingStaffRoleFilters && _interpretedByFilterComponent.IsStarted)
			{
				_worklistDetail.InterpretedByStaff.Staff = new List<StaffSummary>(_interpretedByFilterComponent.SelectedItems);
				_worklistDetail.InterpretedByStaff.IncludeCurrentUser = _interpretedByFilterComponent.IncludeCurrentUser;
			}

			if (ShowReportingStaffRoleFilters && _transcribedByFilterComponent.IsStarted)
			{
				_worklistDetail.TranscribedByStaff.Staff = new List<StaffSummary>(_transcribedByFilterComponent.SelectedItems);
				_worklistDetail.TranscribedByStaff.IncludeCurrentUser = _transcribedByFilterComponent.IncludeCurrentUser;
			}

			if (ShowReportingStaffRoleFilters && _verifiedByFilterComponent.IsStarted)
			{
				_worklistDetail.VerifiedByStaff.Staff = new List<StaffSummary>(_verifiedByFilterComponent.SelectedItems);
				_worklistDetail.VerifiedByStaff.IncludeCurrentUser = _verifiedByFilterComponent.IncludeCurrentUser;
			}

			if (ShowReportingStaffRoleFilters && _supervisedByFilterComponent.IsStarted)
			{
				_worklistDetail.SupervisedByStaff.Staff = new List<StaffSummary>(_supervisedByFilterComponent.SelectedItems);
				_worklistDetail.SupervisedByStaff.IncludeCurrentUser = _supervisedByFilterComponent.IncludeCurrentUser;
			}
		}

		private void AddWorklists()
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					if (_detailComponent is WorklistMultiDetailEditorComponent)
					{
						// add each worklist in the multi editor
						var detailEditor = (WorklistMultiDetailEditorComponent)_detailComponent;
						foreach (var entry in detailEditor.WorklistsToCreate)
						{
							_worklistDetail.Name = entry.Name;
							_worklistDetail.Description = entry.Description;
							_worklistDetail.WorklistClass = entry.Class;

							var response = service.AddWorklist(new AddWorklistRequest(_worklistDetail, !_adminMode));
							_editedWorklistSummaries.Add(response.WorklistAdminSummary);
						}
					}
					else
					{
						// only 1 worklist to add
						var response = service.AddWorklist(new AddWorklistRequest(_worklistDetail, !_adminMode));
						_editedWorklistSummaries.Add(response.WorklistAdminSummary);
					}

				});
		}

		private void UpdateWorklist()
		{
			Platform.GetService(
				delegate(IWorklistAdminService service)
				{
					var response = service.UpdateWorklist(new UpdateWorklistRequest(_worklistRef, _worklistDetail));
					_worklistRef = response.WorklistAdminSummary.WorklistRef;
					_editedWorklistSummaries.Add(response.WorklistAdminSummary);
				});
		}
	}
}
