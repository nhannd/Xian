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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Authority Groups")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.AuthorityGroupAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class AuthorityGroupSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    AuthorityGroupSummaryComponent component = new AuthorityGroupSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleAuthorityGroup);
                    _workspace.Closed += delegate { _workspace = null; };
                }
                catch (Exception e)
                {
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

        private SimpleActionModel _authorityGroupActionHandler;
        private readonly string _addAuthorityGroupKey = "AddAuthorityGroup";
        private readonly string _updateAuthorityGroupKey = "UpdateAuthorityGroup";

        private IPagingController<AuthorityGroupSummary> _pagingController;

        public override void Start()
        {
            _authorityGroupTable = new AuthorityGroupTable();

            _authorityGroupActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _authorityGroupActionHandler.AddAction(_addAuthorityGroupKey, SR.TitleAddAuthorityGroup, "Icons.AddToolSmall.png", SR.TitleAddAuthorityGroup, AddAuthorityGroup);
            _authorityGroupActionHandler.AddAction(_updateAuthorityGroupKey, SR.TitleUpdateAuthorityGroup, "Icons.EditToolSmall.png", SR.TitleUpdateAuthorityGroup, UpdateSelectedAuthorityGroup);

            InitialisePaging(_authorityGroupActionHandler);

            LoadAuthorityGroupTable();

            base.Start();
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<AuthorityGroupSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListAuthorityGroupsResponse listResponse = null;

                    Platform.GetService<IAuthenticationAdminService>(
                        delegate(IAuthenticationAdminService service)
                        {
                            ListAuthorityGroupsRequest listRequest = new ListAuthorityGroupsRequest();
                            listRequest.Page.FirstRow = firstRow;
                            listRequest.Page.MaxRows = maxRows;

                            listResponse = service.ListAuthorityGroups(listRequest);
                        });

                    return listResponse.AuthorityGroups;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<AuthorityGroupSummary>(_pagingController, _authorityGroupTable, Host.DesktopWindow));
            }
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

        private void LoadAuthorityGroupTable()
        {
            _authorityGroupTable.Items.Clear();
            _authorityGroupTable.Items.AddRange(_pagingController.GetFirst());
        }

        public void AddAuthorityGroup()
        {
            try
            {
                AuthorityGroupEditorComponent editor = new AuthorityGroupEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddAuthorityGroup);

                LoadAuthorityGroupTable();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedAuthorityGroup()
        {
            try
            {
                if (_selectedAuthorityGroup == null) return;

                AuthorityGroupEditorComponent editor = new AuthorityGroupEditorComponent(_selectedAuthorityGroup.EntityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateAuthorityGroup);

                LoadAuthorityGroupTable();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void AuthorityGroupSelectionChanged()
        {
            if (_selectedAuthorityGroup != null)
                _authorityGroupActionHandler[_updateAuthorityGroupKey].Enabled = true;
            else
                _authorityGroupActionHandler[_updateAuthorityGroupKey].Enabled = false;
        }
    }
}
