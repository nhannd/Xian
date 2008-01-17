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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Common.Utilities;

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

        private ProcedureTypeGroupSummaryTable _availableProcedureTypeGroups;
        private ProcedureTypeGroupSummaryTable _selectedProcedureTypeGroups;
        private event EventHandler _availableItemsChanged;

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
            _availableProcedureTypeGroups = new ProcedureTypeGroupSummaryTable();
            _selectedProcedureTypeGroups = new ProcedureTypeGroupSummaryTable();

            _availableUsers = new UserTable();
            _selectedUsers = new UserTable();

            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    GetWorklistEditFormDataRequest formDataRequest = new GetWorklistEditFormDataRequest();
                    GetWorklistEditFormDataResponse formDataResponse = service.GetWorklistEditFormData(formDataRequest);

                    _typeChoices = formDataResponse.WorklistTypes;
                    _availableUsers.Items.AddRange(formDataResponse.Users);

                    if (_isNew)
                    {
                        _editedItemDetail = new WorklistAdminDetail();
                        _editedItemDetail.WorklistType = _typeChoices[0];
                    }
                    else
                    {
                        LoadWorklistForEditResponse response =
                            service.LoadWorklistForEdit(new LoadWorklistForEditRequest(_editedItemEntityRef));

                        _editedItemDetail = response.Detail;

                        _selectedProcedureTypeGroups.Items.AddRange(_editedItemDetail.ProcedureTypeGroups);
                        _selectedUsers.Items.AddRange(_editedItemDetail.Users);
                    }

                    ListProcedureTypeGroupsForWorklistCategoryRequest groupsRequest = new ListProcedureTypeGroupsForWorklistCategoryRequest(_editedItemDetail.WorklistType);
                    ListProcedureTypeGroupsForWorklistCategoryResponse groupsResponse = service.ListProcedureTypeGroupsForWorklistCategory(groupsRequest);
                    _availableProcedureTypeGroups.Items.AddRange(groupsResponse.ProcedureTypeGroups);

                    foreach (ProcedureTypeGroupSummary selectedSummary in _selectedProcedureTypeGroups.Items)
                    {
                        _availableProcedureTypeGroups.Items.Remove(selectedSummary);
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
                if (_editedItemDetail.WorklistType != value)
                {
                    _editedItemDetail.WorklistType = value;
                    OnWorklistTypeChanged();
                    this.Modified = true;
                }
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

        public ITable AvailableProcedureTypeGroups
        {
            get { return _availableProcedureTypeGroups; }
        }

        public ITable SelectedProcedureTypeGroups
        {
            get { return _selectedProcedureTypeGroups; }
        }

        public event EventHandler AvailableItemsChanged
        {
            add { _availableItemsChanged += value; }
            remove { _availableItemsChanged -= value; }
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
                    _editedItemDetail.ProcedureTypeGroups.Clear();
                    _editedItemDetail.ProcedureTypeGroups.AddRange(_selectedProcedureTypeGroups.Items);

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

                    this.Exit(ApplicationComponentExitCode.Accepted);
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
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }

        public void OnWorklistTypeChanged()
        {
            try
            {
                // Update AvailableProcedureTypeGroups
                Platform.GetService<IWorklistAdminService>(
                    delegate(IWorklistAdminService service)
                        {
                            ListProcedureTypeGroupsForWorklistCategoryRequest request = new ListProcedureTypeGroupsForWorklistCategoryRequest(_editedItemDetail.WorklistType);
                            ListProcedureTypeGroupsForWorklistCategoryResponse response =
                                service.ListProcedureTypeGroupsForWorklistCategory(request);

                            _availableProcedureTypeGroups.Items.Clear();
                            _availableProcedureTypeGroups.Items.AddRange(response.ProcedureTypeGroups);

                            foreach(ProcedureTypeGroupSummary selectedItem in _selectedProcedureTypeGroups.Items)
                            {
                                _availableProcedureTypeGroups.Items.Remove(selectedItem);
                            }

                            EventsHelper.Fire(_availableItemsChanged, this, EventArgs.Empty);
                        });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }
    }
}
