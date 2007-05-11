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
    //[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class UserSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                UserSummaryComponent component = new UserSummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleUser,
                    delegate(IApplicationComponent c) { _workspace = null; });
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

        private ActionModelRoot _userActionHandler;
        private ClickAction _addUserAction;
        private ClickAction _editUserAction;

        private IPagingController<UserSummary> _pagingController;
        private PagingActionModel<UserSummary> _pagingActionHandler;

        private ListUsersRequest _listRequest;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserSummaryComponent()
        {
        }

        public override void Start()
        {
            _userTable = new UserTable();
            _userActionHandler = new ActionModelRoot();
            _addUserAction = CreateAction(SR.TitleAddUser, "Icons.Add.png", AddUser);
            _editUserAction = CreateAction(SR.TitleUpdateUser, "Icons.Edit.png", UpdateSelectedUser);
            _userActionHandler.InsertAction(_addUserAction);
            _userActionHandler.InsertAction(_editUserAction);

            InitialisePaging();
            _userActionHandler.Merge(_pagingActionHandler);

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<UserSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListUsersResponse listResponse = null;

                    try
                    {
                        Platform.GetService<IAuthenticationAdminService>(
                            delegate(IAuthenticationAdminService service)
                            {
                                ListUsersRequest listRequest = _listRequest;
                                listRequest.PageRequest.FirstRow = firstRow;
                                listRequest.PageRequest.MaxRows = maxRows;

                                listResponse = service.ListUsers(listRequest);
                            });

                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Host.DesktopWindow);
                    }

                    return listResponse.Users;
                }
            );

            _pagingActionHandler = new PagingActionModel<UserSummary>(_pagingController, _userTable);
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITable Users
        {
            get { return _userTable; }
        }

        public ActionModelNode UserListActionModel
        {
            get { return _userActionHandler; }
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
            UserEditorComponent editor = new UserEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddUser);
        }

        public void UpdateSelectedUser()
        {
            if (_selectedUser == null) return;

            UserEditorComponent editor = new UserEditorComponent(_selectedUser.EntityRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateUser);
        }

        public void LoadUserTable()
        {
            _listRequest = new ListUsersRequest();

            _userTable.Items.Clear();
            _userTable.Items.AddRange(_pagingController.GetFirst());
        }

        #endregion

        private void UserSelectionChanged()
        {
            if (_selectedUser != null)
                _editUserAction.Enabled = true;
            else
                _editUserAction.Enabled = false;
        }

        private ClickAction CreateAction(string name, string icon, ClickHandlerDelegate handler)
        {
            IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
            ClickAction action = new ClickAction(name, new ActionPath(string.Format("root/{0}", name), resolver), ClickActionFlags.None, resolver);
            action.Tooltip = name;
            action.Label = name;
            action.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            action.Enabled = true;
            action.SetClickHandler(handler);

            return action;
        }
    }
}
