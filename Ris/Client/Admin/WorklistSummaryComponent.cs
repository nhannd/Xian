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
    public class WorklistSummaryComponent : SummaryComponentBase<WorklistAdminSummary, WorklistAdminSummaryTable>
    {
        private readonly object _duplicateWorklistActionKey = new object();

        public override void Start()
        {
            base.Start();

            // add a "duplicate worklist" action 
            //TODO: change the icon
            this.ActionModel.AddAction(_duplicateWorklistActionKey, SR.TitleDuplicate, "Icons.EditToolSmall.png", DuplicateWorklist);
        }

        #region Presentation Model

        public void DuplicateWorklist()
        {
            try
            {
                if (this.SelectedItems.Count != 1) return;

                WorklistAdminSummary worklist = CollectionUtils.FirstElement(this.SelectedItems);
                WorklistEditorComponent editor = new WorklistEditorComponent(worklist.EntityRef, true);
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
                    new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Medium));

                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    this.Table.Items.AddRange(editor.EditedWorklistSummaries);
                    this.SummarySelection = new Selection(editor.EditedWorklistSummaries);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }
        
        #endregion

        protected override bool SupportsDelete
        {
            get { return true; }
        }

        protected override void OnSelectedItemsChanged()
        {
 	        base.OnSelectedItemsChanged();
            this.ActionModel[_duplicateWorklistActionKey].Enabled = this.SelectedItems.Count == 1;
        }

        protected override IList<WorklistAdminSummary> ListItems(int firstItem, int maxItems)
        {
            ListWorklistsResponse listResponse = null;

            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    ListWorklistsRequest listRequest = new ListWorklistsRequest();
                    listRequest.Page.FirstRow = firstItem;
                    listRequest.Page.MaxRows = maxItems;

                    listResponse = service.ListWorklists(listRequest);
                });

            return listResponse.WorklistSummaries;
        }

        protected override bool AddItems(out IList<WorklistAdminSummary> addedItems)
        {
            WorklistEditorComponent editor = new WorklistEditorComponent();
            ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
                new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Medium));

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                addedItems = editor.EditedWorklistSummaries;
                return true;
            }
            else
            {
                addedItems = null;
                return false;
            }
        }

        protected override bool EditItems(IList<WorklistAdminSummary> items, out IList<WorklistAdminSummary> editedItems)
        {
            WorklistAdminSummary worklist = CollectionUtils.FirstElement(items);
            WorklistEditorComponent editor = new WorklistEditorComponent(worklist.EntityRef, false);
            ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
                new DialogBoxCreationArgs(editor, SR.TitleUpdateWorklist, null, DialogSizeHint.Medium));

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                editedItems = editor.EditedWorklistSummaries;
                return true;
            }
            else
            {
                editedItems = null;
                return false;
            }
        }

        protected override bool DeleteItems(IList<WorklistAdminSummary> items)
        {
            foreach (WorklistAdminSummary worklist in items)
            {
                Platform.GetService<IWorklistAdminService>(
                    delegate(IWorklistAdminService service)
                    {
                        service.DeleteWorklist(new DeleteWorklistRequest(worklist.EntityRef));
                    });
            }
            return true;
        }

        protected override bool IsSameItem(WorklistAdminSummary x, WorklistAdminSummary y)
        {
            return x.EntityRef.Equals(y.EntityRef, true);
        }
    }
}
