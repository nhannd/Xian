#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistSummaryComponent class
    /// </summary>
    [AssociateView(typeof(WorklistSummaryComponentViewExtensionPoint))]
    public class WorklistSummaryComponent : ApplicationComponent
    {
        private WorklistAdminSummary _selectedWorklist;
        private WorklistAdminSummaryTable _worklistAdminSummaryTable;

        private SimpleActionModel _worklistActionHandler;
        private readonly string _addWorklistKey = "AddWorklist";
        private readonly string _updateWorklistKey = "UpdateWorklist";

        private IPagingController<WorklistAdminSummary> _pagingController;

        public override void Start()
        {
            _worklistAdminSummaryTable = new WorklistAdminSummaryTable();

            _worklistActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _worklistActionHandler.AddAction(_addWorklistKey, SR.TitleAddWorklist, "Icons.AddToolSmall.png", SR.TitleAddWorklist, AddWorklist);
            _worklistActionHandler.AddAction(_updateWorklistKey, SR.TitleUpdateWorklist, "Icons.EditToolSmall.png", SR.TitleUpdateWorklist, UpdateWorklist);
            _worklistActionHandler[_addWorklistKey].Enabled = true;
            _worklistActionHandler[_updateWorklistKey].Enabled = false;

            InitialisePaging(_worklistActionHandler);

            LoadWorklistTable();

            base.Start();
        }

        private void InitialisePaging(ActionModelNode actionModelNode)
        {
            _pagingController = new PagingController<WorklistAdminSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListWorklistsResponse listResponse = null;

                    Platform.GetService<IWorklistAdminService>(
                        delegate(IWorklistAdminService service)
                        {
                            ListWorklistsRequest listRequest = new ListWorklistsRequest();
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListWorklists(listRequest);
                        });

                    return listResponse.WorklistSummaries;
                }
            );

            if (actionModelNode != null)
            {
                actionModelNode.Merge(new PagingActionModel<WorklistAdminSummary>(_pagingController, _worklistAdminSummaryTable, Host.DesktopWindow));
            }
        }

        private void LoadWorklistTable()
        {
            _worklistAdminSummaryTable.Items.Clear();
            _worklistAdminSummaryTable.Items.AddRange(_pagingController.GetFirst());
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITable Worklists
        {
            get { return _worklistAdminSummaryTable; }
        }

        public ActionModelNode WorklistActionModel
        {
            get { return _worklistActionHandler; }
        }

        public ISelection SelectedWorklist
        {
            get
            {
                return _selectedWorklist == null
                    ? Selection.Empty
                    : new Selection(_selectedWorklist);
            }
            set
            {
                _selectedWorklist = (WorklistAdminSummary) value.Item;
                SelectedWorklistChanged();
            }
        }

        private void SelectedWorklistChanged()
        {
            _worklistActionHandler[_updateWorklistKey].Enabled = 
                _selectedWorklist != null;
        }

        #endregion

        #region Action Model Handlers

        private void AddWorklist()
        {
            try
            {
                WorklistEditorComponent editor = new WorklistEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddWorklist);

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    LoadWorklistTable();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateWorklist()
        {
            try
            {
                if (_selectedWorklist == null) return;

                WorklistEditorComponent editor = new WorklistEditorComponent(_selectedWorklist.EntityRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateWorklist);

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    LoadWorklistTable();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion
    }
}
