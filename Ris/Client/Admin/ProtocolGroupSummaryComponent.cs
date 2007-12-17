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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Protocol Groups", "Launch")]
    [ActionPermission("launch", AuthorityTokens.ProtocolGroupAdmin)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ProtocolGroupSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    ProtocolGroupSummaryComponent component = new ProtocolGroupSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleProtocolGroups);
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
    /// Extension point for views onto <see cref="ProtocolGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolGroupSummaryComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolGroupSummaryComponentViewExtensionPoint))]
    public class ProtocolGroupSummaryComponent : ApplicationComponent
    {
        #region Private fields

        private ProtocolGroupSummary _selectedProtocolGroup;
        private readonly ProtocolGroupTable _protocolGroupTable;
        private readonly CrudActionModel _protocolGroupActionHandler;

        private PagingController<ProtocolGroupSummary> _pagingController;
        private PagingActionModel<ProtocolGroupSummary> _pagingActionHandler;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolGroupSummaryComponent()
        {
            _protocolGroupTable = new ProtocolGroupTable();
            _protocolGroupActionHandler = new CrudActionModel();
        }

        #endregion

        #region ApplicationComponent overrides

        public override void Start()
        {
            _protocolGroupActionHandler.Add.SetClickHandler(AddProtocolGroup);
            _protocolGroupActionHandler.Edit.SetClickHandler(EditSelectedProtocolGroup);
            _protocolGroupActionHandler.Add.Enabled = true;
            _protocolGroupActionHandler.Edit.Enabled = false;
            _protocolGroupActionHandler.Delete.Visible = false;

            InitialisePaging();
            _protocolGroupActionHandler.Merge(_pagingActionHandler);

            LoadProtocolGroupTable();

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region ActionModel handlers

        public void AddProtocolGroup()
        {
            try
            {
                ProtocolGroupEditorComponent editor = new ProtocolGroupEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddProtocolGroup);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _protocolGroupTable.Items.Add(_selectedProtocolGroup = editor.ProtocolGroupSummary);
                    ProtocolGroupSelectionChanged();
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void EditSelectedProtocolGroup()
        {
            try
            {
                ProtocolGroupEditorComponent editor = new ProtocolGroupEditorComponent(_selectedProtocolGroup.EntityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleEditProtocolGroup);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _protocolGroupTable.Items.Replace(
                        delegate(ProtocolGroupSummary s) { return s.EntityRef.Equals(editor.ProtocolGroupSummary.EntityRef); },
                        editor.ProtocolGroupSummary);
                    _selectedProtocolGroup = editor.ProtocolGroupSummary;
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        #region Presentation Model

        public ITable ProtocolGroups
        {
            get { return _protocolGroupTable; }
        }

        public ActionModelNode ProtocolGroupListActionModel
        {
            get { return _protocolGroupActionHandler; }
        }

        public ISelection SelectedProtocolGroup
        {
            get { return new Selection(_selectedProtocolGroup); }
            set
            {
                _selectedProtocolGroup = (ProtocolGroupSummary) value.Item;
                ProtocolGroupSelectionChanged();
            }
        }

        #endregion

        #region private methods

        private void InitialisePaging()
        {
            _pagingController = new PagingController<ProtocolGroupSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListProtocolGroupsResponse listResponse = null;

                    Platform.GetService<IProtocolAdminService>(
                        delegate(IProtocolAdminService service)
                        {
                            ListProtocolGroupsRequest listRequest = new ListProtocolGroupsRequest();
                            listRequest.Page.FirstRow = firstRow;
                            listRequest.Page.MaxRows = maxRows;

                            listResponse = service.ListProtocolGroups(listRequest);
                        });

                    return listResponse.ProtocolGroups;
                }
            );

            _pagingActionHandler = new PagingActionModel<ProtocolGroupSummary>(_pagingController, _protocolGroupTable, Host.DesktopWindow);
        }

        private void LoadProtocolGroupTable()
        {
            _protocolGroupTable.Items.Clear();
            _protocolGroupTable.Items.AddRange(_pagingController.GetFirst());
        }

        private void ProtocolGroupSelectionChanged()
        {
            _protocolGroupActionHandler.Edit.Enabled = _selectedProtocolGroup != null;
        }

        #endregion
    }
}
