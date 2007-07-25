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
    [ExtensionPoint]
    class TechnologistWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
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
        //something here

        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        event EventHandler SelectedFolderChanged;
        IDesktopWindow DesktopWindow { get; }
    }

    public class TechnologistWorkflowFolderSystem : WorkflowFolderSystem<ModalityWorklistItem>
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

            #region ITechnologistWorkflowItemToolContext Members

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
        //private Folders.SearchFolder _searchFolder;

        public TechnologistWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            : base(folderExplorer)
        {
            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;

            //TODO:  Add folders;
            //this.AddFolder(_searchFolder = new Folders.SearchFolder(this));
            this.AddFolder(new Folders.ScheduledTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.CheckedInTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.InProgressTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.SuspendedTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.CancelledTechnologistWorkflowFolder(this));
            this.AddFolder(new Folders.CompletedTechnologistWorkflowFolder(this));

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    ListWorklistsResponse response = service.ListWorklists(new ListWorklistsRequest());
                    foreach (WorklistSummary worklistSummary in response.Worklists)
                    {
                        WorkflowFolder<ModalityWorklistItem> folder = FolderFactory.Instance.GetFolder(worklistSummary.Type, this, worklistSummary);
                        if (folder != null) this.AddFolder(folder);
                    }
                });

            _itemToolSet = new ToolSet(new TechnologistWorkflowItemToolExtensionPoint(), new TechnologistWorkflowItemToolContext(this));
            _folderToolSet = new ToolSet(new TechnologistWorkflowFolderToolExtensionPoint(), new TechnologistWorkflowFolderToolContext(this));

            folderExplorer.AddItemActions(_itemToolSet.Actions);
            folderExplorer.AddFolderActions(_folderToolSet.Actions);
        }

        public bool GetOperationEnablement(string operationName)
        {
            try
            {
                return _workflowEnablement == null ? false : _workflowEnablement[operationName];
            }
            catch (KeyNotFoundException e)
            {
                Platform.Log(string.Format(SR.ExceptionOperationEnablementUnknown, operationName), LogLevel.Error);
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
