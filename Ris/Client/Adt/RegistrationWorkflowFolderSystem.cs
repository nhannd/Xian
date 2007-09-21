using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Adt.Folders;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class RegistrationWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint]
    public class RegistrationWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IRegistrationWorkflowItemToolContext : IToolContext
    {
        bool GetWorkflowOperationEnablement(string operationClass);

        ICollection<RegistrationWorklistItem> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        IDesktopWindow DesktopWindow { get; }
    }

    public interface IRegistrationWorkflowFolderToolContext : IToolContext
    {
        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        event EventHandler SelectedFolderChanged;
        IDesktopWindow DesktopWindow { get; }
    }

    public class RegistrationWorkflowFolderSystem : WorkflowFolderSystem<RegistrationWorklistItem>, ISearchDataHandler
    {
        class RegistrationWorkflowItemToolContext : ToolContext, IRegistrationWorkflowItemToolContext
        {
            private RegistrationWorkflowFolderSystem _owner;

            public RegistrationWorkflowItemToolContext(RegistrationWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IRegistrationWorkflowItemToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public ICollection<RegistrationWorklistItem> SelectedItems
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

        class RegistrationWorkflowFolderToolContext : ToolContext, IRegistrationWorkflowFolderToolContext
        {
            private RegistrationWorkflowFolderSystem _owner;

            public RegistrationWorkflowFolderToolContext(RegistrationWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IRegistrationWorkflowFolderToolContext Members

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
        private IDictionary<string, bool> _workflowEnablment;
        private Folders.RegistrationSearchFolder _searchFolder;

        public RegistrationWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            : base(folderExplorer, new RegistrationContainerFolderExtensionPoint())
        {
            // important to initialize service before adding any folders, because folders may access service

            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;
            this.SelectedItemDoubleClicked += SelectedItemDoubleClickedEventHandler;

            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    ListWorklistsResponse response = service.ListWorklists(new ListWorklistsRequest());
                    foreach (WorklistSummary worklistSummary in response.Worklists)
                    {
                        WorkflowFolder<RegistrationWorklistItem> folder =
                            WorkflowFolderFactory.Instance.GetFolder<RegistrationWorklistItem>(worklistSummary.Type, this, worklistSummary);
                        if (folder != null) this.AddFolder(folder);
                    }
                });

            this.AddFolder(new Folders.ScheduledFolder(this));
            this.AddFolder(new Folders.CheckedInFolder(this));
            this.AddFolder(new Folders.InProgressFolder(this));
            this.AddFolder(new Folders.CompletedFolder(this));
            this.AddFolder(new Folders.CancelledFolder(this));
            this.AddFolder(_searchFolder = new Folders.RegistrationSearchFolder(this));

            _itemToolSet = new ToolSet(new RegistrationWorkflowItemToolExtensionPoint(), new RegistrationWorkflowItemToolContext(this));
            _folderToolSet = new ToolSet(new RegistrationWorkflowFolderToolExtensionPoint(), new RegistrationWorkflowFolderToolContext(this));

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
                       SearchField.GivenName;
            }
        }

        public bool GetOperationEnablement(string operationName)
        {
            return _workflowEnablment == null ? false : _workflowEnablment[operationName];
        }

        private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            RegistrationWorklistItem selectedItem = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.SelectedItems);
            if (selectedItem == null)
            {
                _workflowEnablment = null;
                return;
            }

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

        private void SelectedItemDoubleClickedEventHandler(object sender, EventArgs e)
        {
            PatientOverviewTool tool = new PatientOverviewTool();
            tool.SetContext(new RegistrationWorkflowItemToolContext(this));
            if (tool.Enabled)
                tool.View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if(_itemToolSet != null) _itemToolSet.Dispose();
                if (_folderToolSet != null) _folderToolSet.Dispose();
            }
        }

    }
}