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
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using System.Collections.Generic;

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
        private IList<WorklistAdminSummary> _selectedWorklists;
        private WorklistAdminSummaryTable _worklistAdminSummaryTable;

        private CrudActionModel _worklistActionModel;
        private readonly object _duplicateWorklistActionKey = new object();

        private IPagingController<WorklistAdminSummary> _pagingController;

        public override void Start()
        {
            _worklistAdminSummaryTable = new WorklistAdminSummaryTable();
            _selectedWorklists = new List<WorklistAdminSummary>();

            _worklistActionModel = new CrudActionModel(true, true, true);
            _worklistActionModel.Add.SetClickHandler(AddWorklist);
            _worklistActionModel.Edit.SetClickHandler(UpdateWorklist);
            _worklistActionModel.Delete.SetClickHandler(DeleteWorklist);

            //TODO: change the icon
            _worklistActionModel.AddAction(_duplicateWorklistActionKey, SR.TitleDuplicate, "Icons.EditToolSmall.png", DuplicateWorklist);

            InitialisePaging(_worklistActionModel);

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
                            listRequest.Page.FirstRow = firstRow;
                            listRequest.Page.MaxRows = maxRows;

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

        #region Presentation Model

        public ITable Worklists
        {
            get { return _worklistAdminSummaryTable; }
        }

        public ActionModelNode WorklistActionModel
        {
            get { return _worklistActionModel; }
        }

        public ISelection SelectedWorklist
        {
            get
            {
                return new Selection(_selectedWorklists);
            }
            set
            {
                _selectedWorklists = new TypeSafeListWrapper<WorklistAdminSummary>(value.Items);
                SelectedWorklistChanged();
            }
        }

        public void AddWorklist()
        {
            try
            {
                WorklistEditorComponent editor = new WorklistEditorComponent();
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, 
                    new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Medium));

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _worklistAdminSummaryTable.Items.AddRange(editor.EditedWorklistSummaries);
                    _selectedWorklists = new List<WorklistAdminSummary>(editor.EditedWorklistSummaries);
                    NotifyPropertyChanged("SelectedWorklist");
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DuplicateWorklist()
        {
            try
            {
                if (_selectedWorklists.Count != 1) return;

                WorklistAdminSummary worklist = CollectionUtils.FirstElement(_selectedWorklists);
                WorklistEditorComponent editor = new WorklistEditorComponent(worklist.EntityRef, true);
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
                    new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Medium));

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _worklistAdminSummaryTable.Items.AddRange(editor.EditedWorklistSummaries);
                    _selectedWorklists = new List<WorklistAdminSummary>(editor.EditedWorklistSummaries);
                    NotifyPropertyChanged("SelectedWorklist");
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
                if (_selectedWorklists.Count != 1) return;

                WorklistAdminSummary worklist = CollectionUtils.FirstElement(_selectedWorklists);
                WorklistEditorComponent editor = new WorklistEditorComponent(worklist.EntityRef, false);
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
                    new DialogBoxCreationArgs(editor, SR.TitleUpdateWorklist, null, DialogSizeHint.Medium));

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    // only single-select editing is supported, so there is only one item
                    WorklistAdminSummary editedItem = CollectionUtils.FirstElement(editor.EditedWorklistSummaries);
                    _worklistAdminSummaryTable.Items.Replace(
                        delegate(WorklistAdminSummary item) { return item.EntityRef.Equals(editedItem.EntityRef, true); },
                        editedItem);
                    _selectedWorklists = new List<WorklistAdminSummary>(editor.EditedWorklistSummaries);
                    NotifyPropertyChanged("SelectedWorklist");
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DeleteWorklist()
        {
            try
            {
                if (_selectedWorklists.Count == 0) return;

                DialogBoxAction action = this.Host.ShowMessageBox(SR.MessageConfirmDeleteWorklists, MessageBoxActions.YesNo);
                if(action == DialogBoxAction.Yes)
                {
                    foreach (WorklistAdminSummary worklist in _selectedWorklists)
                    {
                        Platform.GetService<IWorklistAdminService>(
                            delegate(IWorklistAdminService service)
                            {
                                service.DeleteWorklist(new DeleteWorklistRequest(worklist.EntityRef));
                            });

                        _worklistAdminSummaryTable.Items.Remove(worklist);
                    }

                    // clear selection
                    _selectedWorklists = new List<WorklistAdminSummary>();
                    NotifyPropertyChanged("SelectedWorklist");
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }


        #endregion

        private void SelectedWorklistChanged()
        {
            _worklistActionModel.Edit.Enabled = _selectedWorklists.Count == 1;
            _worklistActionModel.Delete.Enabled = _selectedWorklists.Count > 0;
            _worklistActionModel[_duplicateWorklistActionKey].Enabled = _selectedWorklists.Count == 1;
        }

    }
}
