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
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Users")]
    [ClickHandler("launch", "Launch")]
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
                        SR.TitleUser,
                        delegate(IApplicationComponent c) { _workspace = null; });

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
        private readonly string _addUserKey = "AddUser";
        private readonly string _updateUserKey = "UpdateUser";

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
			_userActionHandler.AddAction(_addUserKey, SR.TitleAddUser, "Icons.AddToolSmall.png", SR.TitleAddUser, AddUser);
			_userActionHandler.AddAction(_updateUserKey, SR.TitleUpdateUser, "Icons.EditToolSmall.png", SR.TitleUpdateUser, UpdateSelectedUser);

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

                    Platform.GetService<IAuthenticationAdminService>(
                        delegate(IAuthenticationAdminService service)
                        {
                            ListUsersRequest listRequest = new ListUsersRequest();
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

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
                if (exitCode == ApplicationComponentExitCode.Normal)
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

                UserEditorComponent editor = new UserEditorComponent(_selectedUser.EntityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateUser);

                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _userTable.Items.Replace(delegate(UserSummary u) { return u.EntityRef.Equals(editor.UserSummary.EntityRef); }, editor.UserSummary);
                }
            }
            catch (Exception e)
            {
                // could not launch editor
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
            if (_selectedUser != null)
                _userActionHandler[_updateUserKey].Enabled = true;
            else
                _userActionHandler[_updateUserKey].Enabled = false;
        }
    }
}
