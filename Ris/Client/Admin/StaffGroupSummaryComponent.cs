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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Staff Groups", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class StaffGroupAdminTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    StaffGroupSummaryComponent component = new StaffGroupSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        "Staff Groups");
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // failed to launch component
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
    /// Extension point for views onto <see cref="StaffGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffGroupSummaryComponent class
    /// </summary>
    [AssociateView(typeof(StaffGroupSummaryComponentViewExtensionPoint))]
    public class StaffGroupSummaryComponent : ApplicationComponent
    {
        private StaffGroupSummary _selectedStaffGroup;
        private Table<StaffGroupSummary> _staffGroupTable;

        private CrudActionModel _actionModel;
        private PagingController<StaffGroupSummary> _pagingController;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffGroupSummaryComponent()
        {
        }

        public override void Start()
        {
            _staffGroupTable = new Table<StaffGroupSummary>();
            _staffGroupTable.Columns.Add(new TableColumn<StaffGroupSummary, string>("Name",
                delegate(StaffGroupSummary item) { return item.Name; }));
            _staffGroupTable.Columns.Add(new TableColumn<StaffGroupSummary, string>("Description",
                delegate(StaffGroupSummary item) { return item.Description; }));

            _actionModel = new CrudActionModel(true, true, false);
            _actionModel.Add.SetClickHandler(AddStaffGroup);
            _actionModel.Edit.SetClickHandler(EditStaffGroup);

            _pagingController = new PagingController<StaffGroupSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListStaffGroupsResponse listResponse = null;
                    Platform.GetService<IStaffGroupAdminService>(
                        delegate(IStaffGroupAdminService service)
                        {
                            listResponse = service.ListStaffGroups(new ListStaffGroupsRequest(new SearchResultPage(firstRow, maxRows)));
                        });

                    return listResponse.StaffGroups;
                }
            );

            _actionModel.Merge(new PagingActionModel<StaffGroupSummary>(_pagingController, _staffGroupTable, Host.DesktopWindow));

            _staffGroupTable.Items.AddRange(_pagingController.GetFirst());

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ActionModelNode ActionModel
        {
            get { return _actionModel; }
        }

        public ITable StaffGroupTable
        {
            get { return _staffGroupTable; }
        }

        public ISelection SelectedStaffGroup
        {
            get { return new Selection(_selectedStaffGroup); }
            set
            {
                if (!Equals(value.Item, _selectedStaffGroup))
                {
                    _selectedStaffGroup = (StaffGroupSummary)value.Item;

                    UpdateActionModel();
                    NotifyPropertyChanged("SelectedStaffGroup");
                }
            }
        }

        public void AddStaffGroup()
        {
            try
            {
                StaffGroupEditorComponent editor = new StaffGroupEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, "Add Staff Group");
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _staffGroupTable.Items.Add(editor.StaffGroupSummary);
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void EditStaffGroup()
        {
            try
            {
                // can occur if user double clicks while holding control, or double clicks when there is no selection
                if (_selectedStaffGroup == null) return;

                StaffGroupEditorComponent editor = new StaffGroupEditorComponent(_selectedStaffGroup.StaffGroupRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, "Edit Staff Group");
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _staffGroupTable.Items.Replace(
                        delegate(StaffGroupSummary s) { return s.StaffGroupRef.Equals(editor.StaffGroupSummary.StaffGroupRef, true); },
                        editor.StaffGroupSummary);
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void UpdateActionModel()
        {
            _actionModel.Edit.Enabled = (_selectedStaffGroup != null);
            _actionModel.Delete.Enabled = (_selectedStaffGroup != null);
        }
    }
}
