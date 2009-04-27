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

    /// <summary>
    /// WorklistEditorComponent class
    /// </summary>
    public class WorklistEditorComponent : NavigatorComponentContainer
    {
        class ProcedureTypeGroupTable : Table<ProcedureTypeGroupSummary>
        {
            public ProcedureTypeGroupTable()
            {
                this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnName,
                    delegate(ProcedureTypeGroupSummary summary) { return summary.Name; }));
            }
        }

        class LocationTable : Table<LocationSummary>
        {
            public LocationTable()
            {
                this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnName,
                    delegate(LocationSummary summary) { return summary.Name; }));
            }
        }

        class StaffTable : Table<StaffSummary>
        {
            public StaffTable()
            {
                this.Columns.Add(new TableColumn<StaffSummary, string>("Name",
                    delegate(StaffSummary item) { return PersonNameFormat.Format(item.Name); }, 1.0f));
                this.Columns.Add(new TableColumn<StaffSummary, string>("Role",
                    delegate(StaffSummary item) { return item.StaffType.Value; }, 0.5f));
            }
        }

        class StaffGroupTable : Table<StaffGroupSummary>
        {
            public StaffGroupTable()
            {
                this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Name",
                    delegate(StaffGroupSummary item) { return item.Name; }, 1.0f));
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
        private WorklistSelectorEditorComponent<ProcedureTypeGroupSummary, ProcedureTypeGroupTable> _procedureTypeGroupFilterComponent;
        private WorklistSelectorEditorComponent<LocationSummary, LocationTable> _locationFilterComponent;
        private WorklistSelectorEditorComponent<StaffSummary, StaffTable> _staffSubscribersComponent;
        private WorklistSelectorEditorComponent<StaffGroupSummary, StaffGroupTable> _groupSubscribersComponent;
        private WorklistPreviewComponent _previewComponent;

        /// <summary>
        /// Constructor to create new worklist(s).
        /// </summary>
        public WorklistEditorComponent(bool adminMode)
            :this(adminMode, null, null)
        {
        }

        /// <summary>
        /// Constructor to create new worklist(s) with limited choices of class.
        /// </summary>
        public WorklistEditorComponent(bool adminMode, IList<string> worklistClassChoices, string selectedClass)
        {
            _mode = WorklistEditorMode.Add;
            _adminMode = adminMode;
            _worklistClassChoices = worklistClassChoices;
            _initialClassName = selectedClass;
        }

        /// <summary>
        /// Constructor edit or duplicate a worklist.
        /// </summary>
        public WorklistEditorComponent(EntityRef entityRef, WorklistEditorMode editorMode, bool adminMode)
        {
            _mode = editorMode;
            _worklistRef = entityRef;
            _adminMode = adminMode;
        }

        public override void Start()
        {
            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    GetWorklistEditFormDataResponse formDataResponse = service.GetWorklistEditFormData(new GetWorklistEditFormDataRequest());

                    // initialize _worklistDetail depending on add vs edit vs duplicate mode
                    List<ProcedureTypeGroupSummary> procedureTypeGroups = new List<ProcedureTypeGroupSummary>();
                    if (_mode == WorklistEditorMode.Add)
                    {
                        _worklistDetail = new WorklistAdminDetail();
                        _worklistDetail.FilterByWorkingFacility = true; // set this by default (Ticket #1848)

                        // limit class choices if specified
                        if (_worklistClassChoices != null)
                        {
                            formDataResponse.WorklistClasses =
                                CollectionUtils.Select(formDataResponse.WorklistClasses,
                                delegate(WorklistClassSummary wc) { return _worklistClassChoices.Contains(wc.ClassName); });
                        }

                        // establish initial class name
                        _worklistDetail.WorklistClass =
                            CollectionUtils.SelectFirst(formDataResponse.WorklistClasses,
                                delegate(WorklistClassSummary wc) { return wc.ClassName == _initialClassName; });
                    }
                    else
                    {
                        // load the existing worklist
                        LoadWorklistForEditResponse response =
                            service.LoadWorklistForEdit(new LoadWorklistForEditRequest(_worklistRef));

                        _worklistDetail = response.Detail;

                        if (_mode == WorklistEditorMode.Duplicate)
                        {
                            _worklistDetail.EntityRef = null;
                            _worklistDetail.Name = _worklistDetail.Name + " copy";
                        }

                        _worklistRef = response.Detail.EntityRef;

                        // determine initial set of proc type groups, since worklist class already known
                        ListProcedureTypeGroupsResponse groupsResponse = service.ListProcedureTypeGroups(
                            new ListProcedureTypeGroupsRequest(_worklistDetail.WorklistClass.ProcedureTypeGroupClassName));
                        procedureTypeGroups = groupsResponse.ProcedureTypeGroups;
                    }

					// sort worklist classes so they appear alphabetically in editor
					formDataResponse.WorklistClasses = CollectionUtils.Sort(formDataResponse.WorklistClasses,
						delegate(WorklistClassSummary x, WorklistClassSummary y) { return x.DisplayName.CompareTo(y.DisplayName); });

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

                    // create all other pages
                    _filterComponent = new WorklistFilterEditorComponent(_worklistDetail,
                        procedureTypeGroups, formDataResponse.FacilityChoices, formDataResponse.OrderPriorityChoices,
                        formDataResponse.PatientClassChoices);

                    _procedureTypeGroupFilterComponent = new WorklistSelectorEditorComponent<ProcedureTypeGroupSummary, ProcedureTypeGroupTable>(
                        procedureTypeGroups, _worklistDetail.ProcedureTypeGroups, delegate(ProcedureTypeGroupSummary s) { return s.ProcedureTypeGroupRef; });

                    _locationFilterComponent = new WorklistSelectorEditorComponent<LocationSummary, LocationTable>(
                        formDataResponse.PatientLocationChoices, _worklistDetail.PatientLocations, delegate(LocationSummary s) { return s.LocationRef; });

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
                        _staffSubscribersComponent = new WorklistSelectorEditorComponent<StaffSummary, StaffTable>(
							formDataResponse.StaffChoices,
							_worklistDetail.StaffSubscribers,
							delegate(StaffSummary s) { return s.StaffRef; },
							SubscriptionPagesReadOnly);
                        _groupSubscribersComponent = new WorklistSelectorEditorComponent<StaffGroupSummary, StaffGroupTable>(
                            formDataResponse.GroupSubscriberChoices,
							_worklistDetail.GroupSubscribers,
							delegate(StaffGroupSummary s) { return s.StaffGroupRef; },
							SubscriptionPagesReadOnly);
                    }
                });

            // add pages
            this.Pages.Add(new NavigatorPage("NodeWorklist", _detailComponent));
            this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters", _filterComponent));
            this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/NodeProcedureTypeGroups", _procedureTypeGroupFilterComponent));
            this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/NodePatientLocations", _locationFilterComponent));

            if (_worklistDetail.WorklistClass == null || ShowReportingStaffRoleFilters)
            {
                this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeInterpretedBy", _interpretedByFilterComponent));
                this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeTranscribedBy", _transcribedByFilterComponent));
                this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeVerifiedBy", _verifiedByFilterComponent));
                this.Pages.Add(new NavigatorPage("NodeWorklist/NodeFilters/NodeStaff/NodeSupervisedBy", _supervisedByFilterComponent));
            }

            // add the time filter page, if the class supports it (or if the class is not known, in the case of an add)
            if (_worklistDetail.WorklistClass == null || _worklistDetail.WorklistClass.SupportsTimeWindow)
            {
                this.Pages.Add(new NavigatorPage("NodeWorklist/NodeTimeWindow", _timeWindowComponent));
            }

            if (ShowSubscriptionPages)
            {
                this.Pages.Add(new NavigatorPage("NodeWorklist/NodeSubscribers/NodeGroupSubscribers", _groupSubscribersComponent));
                this.Pages.Add(new NavigatorPage("NodeWorklist/NodeSubscribers/NodeStaffSubscribers", _staffSubscribersComponent));
            }
            this.Pages.Add(new NavigatorPage("NodeWorklist/Preview", _previewComponent = new WorklistPreviewComponent(_worklistDetail)));

            this.CurrentPageChanged += WorklistEditorComponent_CurrentPageChanged;

            this.ValidationStrategy = new AllComponentsValidationStrategy();

            base.Start();
        }

        private void WorklistEditorComponent_CurrentPageChanged(object sender, EventArgs e)
        {
            if (this.CurrentPage.Component == _previewComponent)
            {
                UpdateWorklistDetail();
                _previewComponent.Refresh();
            }
        }

        private void ProcedureTypeGroupClassChangedEventHandler(object sender, EventArgs e)
        {
            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    ListProcedureTypeGroupsResponse groupsResponse = service.ListProcedureTypeGroups(
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
                        delegate()
                        {
                            this.Exit(ApplicationComponentExitCode.Error);
                        });
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
            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
					if (_detailComponent is WorklistMultiDetailEditorComponent)
                    {
                        // add each worklist in the multi editor
                        WorklistMultiDetailEditorComponent detailEditor = (WorklistMultiDetailEditorComponent)_detailComponent;
                        foreach (WorklistMultiDetailEditorComponent.WorklistTableEntry entry in detailEditor.WorklistsToCreate)
                        {
                            _worklistDetail.Name = entry.Name;
                            _worklistDetail.Description = entry.Description;
                            _worklistDetail.WorklistClass = entry.Class;

                            AddWorklistResponse response = service.AddWorklist(new AddWorklistRequest(_worklistDetail, !_adminMode));
                            _editedWorklistSummaries.Add(response.WorklistAdminSummary);
                        }
                    }
                    else
                    {
                        // only 1 worklist to add
                        AddWorklistResponse response = service.AddWorklist(new AddWorklistRequest(_worklistDetail, !_adminMode));
                        _editedWorklistSummaries.Add(response.WorklistAdminSummary);
                    }

                });
        }

        private void UpdateWorklist()
        {
            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    UpdateWorklistResponse response =
                        service.UpdateWorklist(new UpdateWorklistRequest(_worklistRef, _worklistDetail));
                    _worklistRef = response.WorklistAdminSummary.WorklistRef;
                    _editedWorklistSummaries.Add(response.WorklistAdminSummary);
                });
        }
    }
}
