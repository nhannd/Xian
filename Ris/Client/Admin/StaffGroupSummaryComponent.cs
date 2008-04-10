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
using ClearCanvas.Common.Utilities;

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

    public class StaffGroupSummaryTable : Table<StaffGroupSummary>
    {
        public StaffGroupSummaryTable()
        {
            this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Name",
                delegate(StaffGroupSummary item) { return item.Name; }));
            this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Description",
                delegate(StaffGroupSummary item) { return item.Description; }));
        }
    }

    /// <summary>
    /// StaffGroupSummaryComponent class
    /// </summary>
    [AssociateView(typeof(StaffGroupSummaryComponentViewExtensionPoint))]
    public class StaffGroupSummaryComponent : SummaryComponentBase<StaffGroupSummary, StaffGroupSummaryTable>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StaffGroupSummaryComponent()
        {
        }

        protected override IList<StaffGroupSummary> ListItems(int firstRow, int maxRows)
        {
            ListStaffGroupsResponse listResponse = null;
            Platform.GetService<IStaffGroupAdminService>(
                delegate(IStaffGroupAdminService service)
                {
                    listResponse = service.ListStaffGroups(new ListStaffGroupsRequest(new SearchResultPage(firstRow, maxRows)));
                });

            return listResponse.StaffGroups;
        }

        protected override bool AddItems(out IList<StaffGroupSummary> addedItems)
        {
            addedItems = new List<StaffGroupSummary>();
            StaffGroupEditorComponent editor = new StaffGroupEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, "Add Staff Group");
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                addedItems.Add(editor.StaffGroupSummary);
                return true;
            }
            return false;
        }

        protected override bool EditItems(IList<StaffGroupSummary> items, out IList<StaffGroupSummary> editedItems)
        {
            editedItems = new List<StaffGroupSummary>();
            StaffGroupSummary item = CollectionUtils.FirstElement(items);

            StaffGroupEditorComponent editor = new StaffGroupEditorComponent(item.StaffGroupRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, "Edit Staff Group");
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                editedItems.Add(editor.StaffGroupSummary);
                return true;
            }
            return false;
        }

        protected override bool DeleteItems(IList<StaffGroupSummary> items)
        {
            throw new NotImplementedException();
        }

        protected override bool IsSameItem(StaffGroupSummary x, StaffGroupSummary y)
        {
            return x.StaffGroupRef.Equals(y.StaffGroupRef, true);
        }
    }
}
