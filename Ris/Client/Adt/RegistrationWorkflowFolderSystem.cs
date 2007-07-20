using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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
        PatientProfileSearchData SearchCriteria { set; }

        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        event EventHandler SelectedFolderChanged;
        IDesktopWindow DesktopWindow { get; }
    }

    public class RegistrationWorkflowFolderSystem : WorkflowFolderSystem<RegistrationWorklistItem>
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

            #region IRegistrationWorkflowItemToolContext Members

            public PatientProfileSearchData SearchCriteria
            {
                set { _owner.SearchCriteria = value as PatientProfileSearchData; }
            }

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
        private Folders.SearchFolder _searchFolder;

        public PatientProfileSearchData SearchCriteria
        {
            get { return (_searchFolder == null ? null : _searchFolder.SearchCriteria); }
            set 
            {
                _searchFolder.SearchCriteria = value;
                SelectedFolder = _searchFolder;
            }
        }

        public RegistrationWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            :base(folderExplorer)
        {
            // important to initialize service before adding any folders, because folders may access service

            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;
            this.SelectedItemDoubleClicked += SelectedItemDoubleClickedEventHandler;

            this.AddFolder(new Folders.ScheduledFolder(this));
            this.AddFolder(new Folders.CheckedInFolder(this));
            this.AddFolder(new Folders.InProgressFolder(this));
            this.AddFolder(new Folders.CompletedFolder(this));
            this.AddFolder(new Folders.CancelledFolder(this));
            this.AddFolder(_searchFolder = new Folders.SearchFolder(this));

            _itemToolSet = new ToolSet(new RegistrationWorkflowItemToolExtensionPoint(), new RegistrationWorkflowItemToolContext(this));
            _folderToolSet = new ToolSet(new RegistrationWorkflowFolderToolExtensionPoint(), new RegistrationWorkflowFolderToolContext(this));

            folderExplorer.AddItemActions(_itemToolSet.Actions);
            folderExplorer.AddFolderActions(_folderToolSet.Actions);
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
                    delegate()
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
