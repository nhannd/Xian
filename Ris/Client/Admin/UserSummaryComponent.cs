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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Users", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.UserAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class UserSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    UserSummaryComponent component = new UserSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleUser);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="UserSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class UserSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// UserSummaryComponent class
    /// </summary>
    [AssociateView(typeof(UserSummaryComponentViewExtensionPoint))]
    public class UserSummaryComponent : ApplicationComponent
    {
        private UserSummary _selectedUser;
        private UserTable _userTable;

        private SimpleActionModel _userActionHandler;
        private Action _addUserAction;
        private Action _updateUserAction;
        private Action _resetPasswordAction;

        private IPagingController<UserSummary> _pagingController;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserSummaryComponent()
        {
        }

        public override void Start()
        {
            _userTable = new UserTable();

            _userActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _addUserAction = _userActionHandler.AddAction("add", SR.TitleAddUser, "Icons.AddToolSmall.png", SR.TitleAddUser, AddUser);
            _updateUserAction = _userActionHandler.AddAction("update", SR.TitleUpdateUser, "Icons.EditToolSmall.png", SR.TitleUpdateUser, UpdateSelectedUser);
            _resetPasswordAction = _userActionHandler.AddAction("resetPassword", SR.TitleResetPassword, "Icons.EditToolSmall.png", SR.TitleResetPassword, ResetUserPassword);

            InitialisePaging(_userActionHandler);

            LoadUserTable();

            base.Start();
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<UserSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListUsersResponse listResponse = null;

                    Platform.GetService<IUserAdminService>(
                        delegate(IUserAdminService service)
                        {
                            ListUsersRequest listRequest = new ListUsersRequest();
                            listRequest.Page.FirstRow = firstRow;
                            listRequest.Page.MaxRows = maxRows;

                            listResponse = service.ListUsers(listRequest);
                        });

                    return listResponse.Users;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<UserSummary>(_pagingController, _userTable, Host.DesktopWindow));
            }
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable Users
        {
            get { return _userTable; }
        }

        public ActionModelNode UserListActionModel
        {
            get { return (ActionModelNode)_userActionHandler; }
        }

        public ISelection SelectedUser
        {
            get { return _selectedUser == null ? Selection.Empty : new Selection(_selectedUser); }
            set
            {
                _selectedUser = (UserSummary)value.Item;
                UserSelectionChanged();
            }
        }

        public void AddUser()
        {
            try
            {
                UserEditorComponent editor = new UserEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddUser);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _userTable.Items.Add(editor.UserSummary);
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedUser()
        {
            try
            {
                if (_selectedUser == null) return;

                UserEditorComponent editor = new UserEditorComponent(_selectedUser.UserName);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateUser);

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _userTable.Items.Replace(delegate(UserSummary u) { return u.UserName == editor.UserSummary.UserName; },
                        editor.UserSummary);
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }
        
        public void ResetUserPassword()
        {
            try
            {
                if (_selectedUser == null) return;

                // confirm this action
                if(this.Host.ShowMessageBox(string.Format("Reset password for user {0}?", _selectedUser.UserName), MessageBoxActions.OkCancel) == DialogBoxAction.Cancel)
                    return;

                Platform.GetService<IUserAdminService>(
                    delegate(IUserAdminService service)
                    {
                        ResetUserPasswordResponse response = service.ResetUserPassword(new ResetUserPasswordRequest(_selectedUser.UserName));
                        _userTable.Items.Replace(delegate(UserSummary u) { return u.UserName == response.UserSummary.UserName; },
                           response.UserSummary);
                    });
            }
            catch (Exception e)
            {
                // failed
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void LoadUserTable()
        {
            _userTable.Items.Clear();
            _userTable.Items.AddRange(_pagingController.GetFirst());
        }

        private void UserSelectionChanged()
        {
            _updateUserAction.Enabled = (_selectedUser != null);
            _resetPasswordAction.Enabled = (_selectedUser != null);
        }
    }
}
