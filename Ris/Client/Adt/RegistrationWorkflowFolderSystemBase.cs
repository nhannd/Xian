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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface IRegistrationWorkflowItemToolContext : IWorkflowItemToolContext<RegistrationWorklistItem>
    {
        RegistrationWorkflowFolderSystemBase FolderSystem { get; }
    }

    public interface IRegistrationWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

    public abstract class RegistrationWorkflowFolderSystemBase : WorkflowFolderSystem<RegistrationWorklistItem>
    {
        class RegistrationWorkflowItemToolContext : ToolContext, IRegistrationWorkflowItemToolContext
        {
            private readonly RegistrationWorkflowFolderSystemBase _owner;

            public RegistrationWorkflowItemToolContext(RegistrationWorkflowFolderSystemBase owner)
            {
                _owner = owner;
            }

            #region IRegistrationWorkflowItemToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public ISelection Selection
            {
                get { return _owner.SelectedItems; }
            }

            public ICollection<RegistrationWorklistItem> SelectedItems
            {
                get
                {
                    return CollectionUtils.Map<object, RegistrationWorklistItem>(_owner.SelectedItems.Items,
                        delegate(object item) { return (RegistrationWorklistItem)item; });
                }
            }

            public event EventHandler SelectionChanged
            {
                add { _owner.SelectedItemsChanged += value; }
                remove { _owner.SelectedItemsChanged -= value; }
            }

            public IEnumerable Folders
            {
                get { return _owner.Folders; }
            }

            public IFolder SelectedFolder
            {
                get { return _owner.SelectedFolder; }
            }

            public bool GetWorkflowOperationEnablement(string operationClass)
            {
                return _owner.GetOperationEnablement(operationClass);
            }

            public RegistrationWorkflowFolderSystemBase FolderSystem
            {
                get { return _owner; }
            }

            #endregion
        }

        class RegistrationWorkflowFolderToolContext : ToolContext, IRegistrationWorkflowFolderToolContext
        {
            private readonly RegistrationWorkflowFolderSystemBase _owner;

            public RegistrationWorkflowFolderToolContext(RegistrationWorkflowFolderSystemBase owner)
            {
                _owner = owner;
            }

            #region IRegistrationWorkflowFolderToolContext Members

            public event EventHandler SelectedFolderChanged
            {
                add { _owner.SelectedFolderChanged += value; }
                remove { _owner.SelectedFolderChanged -= value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public IEnumerable Folders
            {
                get { return _owner.Folders; }
            }

            public IFolder SelectedFolder
            {
                get { return _owner.SelectedFolder; }
            }

            #endregion
        }

        private IDictionary<string, bool> _workflowEnablment;

        public RegistrationWorkflowFolderSystemBase(
            IFolderExplorerToolContext folderExplorer, 
            ExtensionPoint<IFolder> folderExtensionPoint,
            ExtensionPoint<ITool> itemToolExtensionPoint,
            ExtensionPoint<ITool> folderToolExtensionPoint)
            : base(folderExplorer, folderExtensionPoint)
        {
            if (this.WorklistTokens.Count > 0)
            {
                Platform.GetService<IRegistrationWorkflowService>(
                    delegate(IRegistrationWorkflowService service)
                    {
                        ListWorklistsResponse response = service.ListWorklists(new ListWorklistsRequest(this.WorklistTokens));
                        foreach (WorklistSummary summary in response.Worklists)
                        {
                            Type foundType = GetWorklistType(summary.Type);
                            WorkflowFolder<RegistrationWorklistItem> folder = 
                                (WorkflowFolder<RegistrationWorklistItem>)Activator.CreateInstance(foundType, this, summary.DisplayName, summary.Description, summary.EntityRef);
                            if (folder != null)
                            {
                                folder.IsStatic = false;
                                this.AddFolder(folder);
                            }
                        }
                    });
            }

            _itemTools = new ToolSet(itemToolExtensionPoint, new RegistrationWorkflowItemToolContext(this));
            _folderTools = new ToolSet(folderToolExtensionPoint, new RegistrationWorkflowFolderToolContext(this));
        }

        public bool GetOperationEnablement(string operationName)
        {
            try
            {
                return _workflowEnablment == null ? false : _workflowEnablment[operationName];
            }
            catch (KeyNotFoundException)
            {
                Platform.Log(LogLevel.Error, string.Format(SR.ExceptionOperationEnablementUnknown, operationName));
                return false;
            }

        }

        public override void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            RegistrationWorklistItem selectedItem = (RegistrationWorklistItem)this.SelectedItems.Item;

            if (selectedItem == null)
            {
                _workflowEnablment = null;
            }
            else
            {
                try
                {
                    BlockingOperation.Run(
                        delegate
                        {
                            Platform.GetService<IRegistrationWorkflowService>(
                                delegate(IRegistrationWorkflowService service)
                                {
                                    GetOperationEnablementResponse response = service.GetOperationEnablement(new GetOperationEnablementRequest(selectedItem.PatientProfileRef, selectedItem.OrderRef));
                                    _workflowEnablment = response.OperationEnablementDictionary;
                                });
                        });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Report(ex, this.DesktopWindow);
                }
            }

            base.SelectedItemsChangedEventHandler(sender, e);
        }
    }
}