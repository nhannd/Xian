using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistEditorComponentViewExtensionPoint))]
    public class WorklistEditorComponent : ApplicationComponent
    {
        private readonly bool _isNew;

        private EntityRef _editedItemEntityRef;
        private WorklistAdminSummary _editedItemSummary;
        private WorklistAdminDetail _editedItemDetail;

        private List<string> _typeChoices;

        private RequestedProcedureTypeGroupSummaryTable _availableRequestedProcedureTypeGroups;
        private RequestedProcedureTypeGroupSummaryTable _selectedRequestedProcedureTypeGroups;
        private RequestedProcedureTypeGroupSummary _selectedRequestedProcedureTypeGroupSelection;
        private RequestedProcedureTypeGroupSummary _availableRequestedProcedureTypeGroupSelection;

        private UserTable _availableUsers;
        private UserTable _selectedUsers;
        private UserSummary _selectedUserSelection;
        private UserSummary _availableUserSelection;

        private event EventHandler _requestedProcedureTypeGroupSummaryTablesChanged;
        private event EventHandler _userSummaryTablesChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistEditorComponent()
        {
            _isNew = true;
        }

        public WorklistEditorComponent(EntityRef entityRef)
        {
            _isNew = false;
            _editedItemEntityRef = entityRef;
        }

        public override void Start()
        {
            _availableRequestedProcedureTypeGroups = new RequestedProcedureTypeGroupSummaryTable();
            _selectedRequestedProcedureTypeGroups = new RequestedProcedureTypeGroupSummaryTable();

            _availableUsers = new UserTable();
            _selectedUsers = new UserTable();

            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    GetWorklistEditFormDataResponse formDataResponse =
                        service.GetWorklistEditFormData(new GetWorklistEditFormDataRequest());
                    _typeChoices = formDataResponse.WorklistTypes;
                    _availableRequestedProcedureTypeGroups.Items.AddRange(formDataResponse.RequestedProcedureTypeGroups);
                    _availableUsers.Items.AddRange(formDataResponse.Users);

                    if(_isNew)
                    {
                        _editedItemDetail = new WorklistAdminDetail();
                        _editedItemDetail.WorklistType = _typeChoices[0];
                    }
                    else
                    {
                        LoadWorklistForEditResponse response =
                            service.LoadWorklistForEdit(new LoadWorklistForEditRequest(_editedItemEntityRef));

                        _editedItemDetail = response.Detail;
                        _selectedRequestedProcedureTypeGroups.Items.AddRange(_editedItemDetail.RequestedProcedureTypeGroups);
                        _selectedUsers.Items.AddRange(_editedItemDetail.Users);
                    }

                    foreach (RequestedProcedureTypeGroupSummary selectedSummary in _selectedRequestedProcedureTypeGroups.Items)
                    {
                        _availableRequestedProcedureTypeGroups.Items.Remove(selectedSummary);
                    }

                    foreach (UserSummary selectedUserSummary in _selectedUsers.Items)
                    {
                        _availableUsers.Items.Remove(selectedUserSummary);
                    }

                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public WorklistAdminSummary EditedWorklistSummary
        {
            get { return _editedItemSummary; }
        }

        #region Presentation Model

        public string Name
        {
            get { return _editedItemDetail.Name; }
            set
            {
                _editedItemDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return ""; }
            set
            {
                _editedItemDetail.Description = value;
                this.Modified = true;
            }
        }

        #region Type
        public string Type
        {
            get { return _editedItemDetail.WorklistType; }
            set
            {
                _editedItemDetail.WorklistType = value;
                this.Modified = true;
            }
        }

        public IList<string> TypeChoices
        {
            get { return _typeChoices; }
        }

        public bool TypeEnabled
        {
            get { return _isNew; }
        }
        #endregion

        public ITable AvailableRequestedProcedureTypeGroups
        {
            get { return _availableRequestedProcedureTypeGroups; }
        }

        public ITable SelectedRequestedProcedureTypeGroups
        {
            get { return _selectedRequestedProcedureTypeGroups; }
        }

        public ISelection SelectedRequestedProcedureTypeGroupsSelection
        {
            get { return _selectedRequestedProcedureTypeGroupSelection == null ? Selection.Empty : new Selection(_selectedRequestedProcedureTypeGroupSelection); }
            set
            {
                _selectedRequestedProcedureTypeGroupSelection = (RequestedProcedureTypeGroupSummary)value.Item;
            }
        }

        public ISelection AvailableRequestedProcedureTypeGroupsSelection
        {
            get { return _availableRequestedProcedureTypeGroupSelection == null ? Selection.Empty : new Selection(_availableRequestedProcedureTypeGroupSelection); }
            set
            {
                _availableRequestedProcedureTypeGroupSelection = (RequestedProcedureTypeGroupSummary)value.Item;
            }
        }

        public event EventHandler RequestedProcedureTypeGroupTablesChanged
        {
            add { _requestedProcedureTypeGroupSummaryTablesChanged += value; }
            remove { _requestedProcedureTypeGroupSummaryTablesChanged -= value; }
        }

        public ITable AvailableUsers
        {
            get { return _availableUsers; }
        }

        public ITable SelectedUsers
        {
            get { return _selectedUsers; }
        }

        public ISelection SelectedUsersSelection
        {
            get { return _selectedUserSelection == null ? Selection.Empty : new Selection(_selectedUserSelection); }
            set
            {
                _selectedUserSelection = (UserSummary)value.Item;
            }
        }

        public ISelection AvailableUsersSelection
        {
            get { return _availableUserSelection == null ? Selection.Empty : new Selection(_availableUserSelection); }
            set
            {
                _availableUserSelection = (UserSummary)value.Item;
            }
        }

        public event EventHandler UserTablesChanged
        {
            add { _userSummaryTablesChanged += value; }
            remove { _userSummaryTablesChanged -= value; }
        }

        #endregion

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    _editedItemDetail.RequestedProcedureTypeGroups.Clear();
                    _editedItemDetail.RequestedProcedureTypeGroups.AddRange(_selectedRequestedProcedureTypeGroups.Items);

                    _editedItemDetail.Users.Clear();
                    _editedItemDetail.Users.AddRange(_selectedUsers.Items);

                    Platform.GetService<IWorklistAdminService>(
                        delegate(IWorklistAdminService service)
                        {
                            if (_isNew)
                            {
                                AddWorklistResponse response =
                                    service.AddWorklist(new AddWorklistRequest(_editedItemDetail));
                                _editedItemEntityRef = response.WorklistAdminSummary.EntityRef;
                                _editedItemSummary = response.WorklistAdminSummary;
                            }
                            else
                            {
                                UpdateWorklistResponse response =
                                    service.UpdateWorklist(new UpdateWorklistRequest(_editedItemEntityRef, _editedItemDetail));
                                _editedItemEntityRef = response.WorklistAdminSummary.EntityRef;
                                _editedItemSummary = response.WorklistAdminSummary;
                            }
                        });

                    this.Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveWorklist, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        #region RequestedProcedureTypeGroup manipulations

        public bool AddSelectionEnabled
        {
            get { return _availableRequestedProcedureTypeGroupSelection != null; }
        }

        public void AddRequestedProcedureTypeSelection(ISelection selection)
        {
            foreach (RequestedProcedureTypeGroupSummary summary in selection.Items)
            {
                _selectedRequestedProcedureTypeGroups.Items.Add(summary);
                _availableRequestedProcedureTypeGroups.Items.Remove(summary);
            }
            EventsHelper.Fire(_requestedProcedureTypeGroupSummaryTablesChanged, this, EventArgs.Empty);
            this.Modified = true;
        }

        public bool RemoveSelectionEnabled
        {
            get { return _selectedRequestedProcedureTypeGroupSelection != null; }
        }

        public void RemoveRequestedProcedureTypeSelection(ISelection selection)
        {
            foreach (RequestedProcedureTypeGroupSummary summary in selection.Items)
            {
                _selectedRequestedProcedureTypeGroups.Items.Remove(summary);
                _availableRequestedProcedureTypeGroups.Items.Add(summary);
            }
            EventsHelper.Fire(_requestedProcedureTypeGroupSummaryTablesChanged, this, EventArgs.Empty);
            this.Modified = true;
        }

        #endregion

        public bool AddUserSelectionEnabled
        {
            get { return _availableUserSelection != null; }
        }

        public void AddUserSelection(ISelection selection)
        {
            foreach (UserSummary summary in selection.Items)
            {
                _selectedUsers.Items.Add(summary);
                _availableUsers.Items.Remove(summary);
            }
            EventsHelper.Fire(_userSummaryTablesChanged, this, EventArgs.Empty);
            this.Modified = true;
        }

        public bool RemoveUserSelectionEnabled
        {
            get { return _selectedUserSelection != null; }
        }

        public void RemoveUserSelection(ISelection selection)
        {
            foreach (UserSummary summary in selection.Items)
            {
                _selectedUsers.Items.Remove(summary);
                _availableUsers.Items.Add(summary);
            }
            EventsHelper.Fire(_userSummaryTablesChanged, this, EventArgs.Empty);
            this.Modified = true;
        }

    }
}
