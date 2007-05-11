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
    [MenuAction("launch", "global-menus/Admin/Authority Groups")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.AuthorityGroupAdmin)]
    //[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class AuthorityGroupSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                AuthorityGroupSummaryComponent component = new AuthorityGroupSummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleAuthorityGroup,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="AuthorityGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AuthorityGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AuthorityGroupSummaryComponent class
    /// </summary>
    [AssociateView(typeof(AuthorityGroupSummaryComponentViewExtensionPoint))]
    public class AuthorityGroupSummaryComponent : ApplicationComponent
    {
        private AuthorityGroupSummary _selectedAuthorityGroup;
        private AuthorityGroupTable _authorityGroupTable;

        private ActionModelRoot _authorityGroupActionHandler;
        private ClickAction _addAuthorityGroupAction;
        private ClickAction _editAuthorityGroupAction;

        private IPagingController<AuthorityGroupSummary> _pagingController;
        private PagingActionModel<AuthorityGroupSummary> _pagingActionHandler;

        private ListAuthorityGroupsRequest _listRequest;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorityGroupSummaryComponent()
        {
        }

        public override void Start()
        {
            _authorityGroupTable = new AuthorityGroupTable();
            _authorityGroupActionHandler = new ActionModelRoot();
            _addAuthorityGroupAction = CreateAction(SR.TitleAddUser, "Icons.Add.png", AddAuthorityGroup);
            _editAuthorityGroupAction = CreateAction(SR.TitleUpdateAuthorityGroup, "Icons.Edit.png", UpdateSelectedAuthorityGroup);
            _authorityGroupActionHandler.InsertAction(_addAuthorityGroupAction);
            _authorityGroupActionHandler.InsertAction(_editAuthorityGroupAction);

            InitialisePaging();
            _authorityGroupActionHandler.Merge(_pagingActionHandler);

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<AuthorityGroupSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListAuthorityGroupsResponse listResponse = null;

                    Platform.GetService<IAuthenticationAdminService>(
                        delegate(IAuthenticationAdminService service)
                        {
                            ListAuthorityGroupsRequest listRequest = _listRequest;
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListAuthorityGroups(listRequest);
                        });

                    return listResponse.AuthorityGroups;
                }
            );

            _pagingActionHandler = new PagingActionModel<AuthorityGroupSummary>(_pagingController, _authorityGroupTable);
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITable AuthorityGroups
        {
            get { return _authorityGroupTable; }
        }

        public ActionModelNode AuthorityGroupListActionModel
        {
            get { return _authorityGroupActionHandler; }
        }

        public ISelection SelectedAuthorityGroup
        {
            get { return _selectedAuthorityGroup == null ? Selection.Empty : new Selection(_selectedAuthorityGroup); }
            set
            {
                _selectedAuthorityGroup = (AuthorityGroupSummary)value.Item;
                AuthorityGroupSelectionChanged();
            }
        }

        #endregion

        public void LoadAuthorityGroupTable()
        {
            _listRequest = new ListAuthorityGroupsRequest();
            _authorityGroupTable.Items.Clear();
            _authorityGroupTable.Items.AddRange(_pagingController.GetFirst());
        }

        public void AddAuthorityGroup()
        {
            AuthorityGroupEditorComponent editor = new AuthorityGroupEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddAuthorityGroup);

            LoadAuthorityGroupTable();
        }

        public void UpdateSelectedAuthorityGroup()
        {
            if (_selectedAuthorityGroup == null) return;

            AuthorityGroupEditorComponent editor = new AuthorityGroupEditorComponent(_selectedAuthorityGroup.EntityRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleUpdateAuthorityGroup);

            LoadAuthorityGroupTable();
        }

        private void AuthorityGroupSelectionChanged()
        {
            if (_selectedAuthorityGroup != null)
                _editAuthorityGroupAction.Enabled = true;
            else
                _editAuthorityGroupAction.Enabled = false;
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
