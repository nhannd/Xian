using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
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

        private UserTable _availableUsers;
        private UserTable _selectedUsers;

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
            get { return _editedItemDetail.Description; }
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

        public ITable AvailableUsers
        {
            get { return _availableUsers; }
        }

        public ITable SelectedUsers
        {
            get { return _selectedUsers; }
        }

        #endregion

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

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }
    }
}
