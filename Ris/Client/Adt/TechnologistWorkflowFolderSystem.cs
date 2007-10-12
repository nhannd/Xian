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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Adt.Folders;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class TechnologistWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint]
    public class TechnologistWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface ITechnologistWorkflowItemToolContext : IToolContext
    {
        bool GetWorkflowOperationEnablement(string operationClass);

        ICollection<ModalityWorklistItem> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        IDesktopWindow DesktopWindow { get; }
    }

    public interface ITechnologistWorkflowFolderToolContext : IToolContext
    {
        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        event EventHandler SelectedFolderChanged;
        IDesktopWindow DesktopWindow { get; }
    }

    public class TechnologistWorkflowFolderSystem : WorkflowFolderSystem<ModalityWorklistItem>, ISearchDataHandler
    {
        class TechnologistWorkflowItemToolContext : ToolContext, ITechnologistWorkflowItemToolContext
        {
            private TechnologistWorkflowFolderSystem _owner;

            public TechnologistWorkflowItemToolContext(TechnologistWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region ITechnologistWorkflowItemToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public ICollection<ModalityWorklistItem> SelectedItems
            {
                get { return _owner.SelectedItems; }
            }

            public event EventHandler SelectedItemsChanged
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
            private TechnologistWorkflowFolderSystem _owner;

            public TechnologistWorkflowFolderToolContext(TechnologistWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region ITechnologistWorkflowFolderToolContext Members

            public event EventHandler SelectedFolderChanged
            {
                add { _owner.SelectedItemsChanged += value; }
                remove { _owner.SelectedItemsChanged -= value; }
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

        private ToolSet _itemToolSet;
        private ToolSet _folderToolSet;
        private IDictionary<string, bool> _workflowEnablement;
        private Folders.TechnologistSearchFolder _searchFolder;

        public TechnologistWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            : base(folderExplorer, new TechnologistContainerFolderExtensionPoint())
        {
            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    ListWorklistsResponse response = service.ListWorklists(new ListWorklistsRequest());
                    foreach (WorklistSummary worklistSummary in response.Worklists)
                    {
                        WorkflowFolder<ModalityWorklistItem> folder =
                            WorkflowFolderFactory.Instance.GetFolder<ModalityWorklistItem>(worklistSummary.Type, this, worklistSummary);
                        if (folder != null) this.AddFolder(folder);
                    }
                });

            this.AddFolder(new Folders.ScheduledTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.CheckedInTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.InProgressTechnologistWorkflowFolder(this));
            //this.AddFolder(new Folders.SuspendedTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.CancelledTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.CompletedTechnologistWorkflowFolder(this));
            this.AddFolder(_searchFolder = new Folders.TechnologistSearchFolder(this));

            _itemToolSet = new ToolSet(new TechnologistWorkflowItemToolExtensionPoint(), new TechnologistWorkflowItemToolContext(this));
            _folderToolSet = new ToolSet(new TechnologistWorkflowFolderToolExtensionPoint(), new TechnologistWorkflowFolderToolContext(this));

            folderExplorer.AddItemActions(_itemToolSet.Actions);
            folderExplorer.AddFolderActions(_folderToolSet.Actions);

            folderExplorer.RegisterSearchDataHandler(this);
        }

        public SearchData SearchData
        {
            set
            {
                _searchFolder.SearchData = value;
                SelectedFolder = _searchFolder;
            }
        }

        public SearchField SearchFields
        {
            get
            {
                return SearchField.Mrn |
                       SearchField.Healthcard |
                       SearchField.FamilyName |
                       SearchField.GivenName |
                       SearchField.AccessionNumber;
            }
        }

        public bool GetOperationEnablement(string operationName)
        {
            try
            {
                return _workflowEnablement == null ? false : _workflowEnablement[operationName];
            }
            catch (KeyNotFoundException e)
            {
                Platform.Log(LogLevel.Error, string.Format(SR.ExceptionOperationEnablementUnknown, operationName));
                return false;
            }
        }

        private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            ModalityWorklistItem selectedItem = CollectionUtils.FirstElement<ModalityWorklistItem>(this.SelectedItems);

            if (selectedItem == null)
            {
                _workflowEnablement = null;
                return;
            }

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_itemToolSet != null) _itemToolSet.Dispose();
                if (_folderToolSet != null) _folderToolSet.Dispose();
            }
        }
    }
}
