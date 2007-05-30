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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    [ExtensionPoint]
    public class ReportingWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint]
    public class ReportingWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IReportingWorkflowItemToolContext : IToolContext
    {
        bool GetWorkflowOperationEnablement(string operationClass);

        ICollection<ReportingWorklistItem> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        IDesktopWindow DesktopWindow { get; }
    }

    public interface IReportingWorkflowFolderToolContext : IToolContext
    {
        event EventHandler SelectedFolderChanged;
        IDesktopWindow DesktopWindow { get; }
    }

    public class ReportingWorkflowFolderSystem : WorkflowFolderSystem<ReportingWorklistItem>
    {
        class ReportingWorkflowItemToolContext : ToolContext, IReportingWorkflowItemToolContext
        {
            private ReportingWorkflowFolderSystem _owner;

            public ReportingWorkflowItemToolContext(ReportingWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IReportingWorkflowItemToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public ICollection<ReportingWorklistItem> SelectedItems
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

        class ReportingWorkflowFolderToolContext : ToolContext, IReportingWorkflowFolderToolContext
        {
            private ReportingWorkflowFolderSystem _owner;

            public ReportingWorkflowFolderToolContext(ReportingWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IReportingWorkflowItemToolContext Members

            public event EventHandler SelectedFolderChanged
            {
                add { _owner.SelectedItemsChanged += value; }
                remove { _owner.SelectedItemsChanged -= value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            #endregion
        }

        private ToolSet _itemToolSet;
        private ToolSet _folderToolSet;
        private IDictionary<string, bool> _workflowEnablment;

        public ReportingWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            :base(folderExplorer)
        {
            // important to initialize service before adding any folders, because folders may access service

            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;

            this.AddFolder(new Folders.TestFolder(this));

            _itemToolSet = new ToolSet(new ReportingWorkflowItemToolExtensionPoint(), new ReportingWorkflowItemToolContext(this));
            _folderToolSet = new ToolSet(new ReportingWorkflowFolderToolExtensionPoint(), new ReportingWorkflowFolderToolContext(this));

            folderExplorer.AddItemActions(_itemToolSet.Actions);
            folderExplorer.AddFolderActions(_folderToolSet.Actions);
        }

        public bool GetOperationEnablement(string operationName)
        {
            return _workflowEnablment == null ? false : _workflowEnablment[operationName];
        }

        private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            ReportingWorklistItem selectedItem = CollectionUtils.FirstElement<ReportingWorklistItem>(this.SelectedItems);
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
                        Platform.GetService<IReportingWorkflowService>(
                            delegate(IReportingWorkflowService service)
                            {
                                GetOperationEnablementResponse response = service.GetOperationEnablement(new GetOperationEnablementRequest(selectedItem));
                                _workflowEnablment = response.OperationEnablementDictionary;
                            });
                    });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Report(ex, this.DesktopWindow);
            }
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
