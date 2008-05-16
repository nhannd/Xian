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
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface ITechnologistWorkflowItemToolContext : IWorkflowItemToolContext<ModalityWorklistItem>
    {
    }

    public interface ITechnologistWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

    public abstract class TechnologistWorkflowFolderSystemBase : WorkflowFolderSystem<ModalityWorklistItem>
    {
        class TechnologistWorkflowItemToolContext : ToolContext, ITechnologistWorkflowItemToolContext
        {
            private readonly TechnologistWorkflowFolderSystemBase _owner;

            public TechnologistWorkflowItemToolContext(TechnologistWorkflowFolderSystemBase owner)
            {
                _owner = owner;
            }

            #region ITechnologistWorkflowItemToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public ISelection Selection
            {
                get { return _owner.SelectedItems; }
            }

            public ICollection<ModalityWorklistItem> SelectedItems
            {
                get
                {
                    return CollectionUtils.Map<object, ModalityWorklistItem>(_owner.SelectedItems.Items,
                        delegate(object item) { return (ModalityWorklistItem)item; });
                }
            }

            public event EventHandler SelectionChanged
            {
                add { _owner.SelectedItemsChanged += value; }
                remove { _owner.SelectedItemsChanged -= value; }
            }

            public bool GetWorkflowOperationEnablement(string operationClass)
            {
                return _owner.GetOperationEnablement(operationClass);
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

        class TechnologistWorkflowFolderToolContext : ToolContext, ITechnologistWorkflowFolderToolContext
        {
            private readonly TechnologistWorkflowFolderSystemBase _owner;

            public TechnologistWorkflowFolderToolContext(TechnologistWorkflowFolderSystemBase owner)
            {
                _owner = owner;
            }

            #region ITechnologistWorkflowFolderToolContext Members

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

        private IDictionary<string, bool> _workflowEnablement;

        public TechnologistWorkflowFolderSystemBase(
			string displayName,
            IFolderExplorerToolContext folderExplorer,
            ExtensionPoint<IFolder> folderExtensionPoint,
            ExtensionPoint<ITool> itemToolExtensionPoint,
            ExtensionPoint<ITool> folderToolExtensionPoint)
            : base(displayName, folderExplorer, folderExtensionPoint)
        {
            if (this.WorklistClassNames.Count > 0)
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                        {
                            ListWorklistsResponse response =
                                service.ListWorklists(new ListWorklistsRequest(this.WorklistClassNames));
                            foreach (WorklistSummary summary in response.Worklists)
                            {
                                Type foundType = GetFolderClassForWorklistClass(summary.ClassName);
                                WorkflowFolder<ModalityWorklistItem> folder =
                                    (WorkflowFolder<ModalityWorklistItem>)Activator.CreateInstance(foundType, this, summary.DisplayName, summary.Description, summary.WorklistRef);
                                if (folder != null)
                                {
                                    folder.IsStatic = false;
                                    this.AddFolder(folder);
                                }
                            }
                        });
            }

            _itemTools = new ToolSet(itemToolExtensionPoint, new TechnologistWorkflowItemToolContext(this));
            _folderTools = new ToolSet(folderToolExtensionPoint, new TechnologistWorkflowFolderToolContext(this));
        }

        public bool GetOperationEnablement(string operationName)
        {
            try
            {
                return _workflowEnablement == null ? false : _workflowEnablement[operationName];
            }
            catch (KeyNotFoundException)
            {
                Platform.Log(LogLevel.Error, string.Format(SR.ExceptionOperationEnablementUnknown, operationName));
                return false;
            }
        }

        public override void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            ModalityWorklistItem selectedItem = (ModalityWorklistItem) this.SelectedItems.Item;

            if (selectedItem == null)
            {
                _workflowEnablement = null;
            }
            else
            {
                try
                {
                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            GetOperationEnablementResponse response = service.GetOperationEnablement(new GetOperationEnablementRequest(selectedItem.ProcedureStepRef));
                            _workflowEnablement = response.OperationEnablementDictionary;
                        });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.ReferenceEquals(ex, this.DesktopWindow);
                }
            }

            base.SelectedItemsChangedEventHandler(sender, e);
        }
    }
}
