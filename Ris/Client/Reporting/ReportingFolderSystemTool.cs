using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Reporting
{
    [ExtensionPoint]
    public class ReportingFolderExplorerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionOf(typeof(ReportingFolderExplorerToolExtensionPoint))]
    public class ReportingWorkflowFolderSystemTool : Tool<IFolderExplorerToolContext>
    {
        private ReportingWorkflowFolderSystem _folderSystem;

        public ReportingWorkflowFolderSystemTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _folderSystem = new ReportingWorkflowFolderSystem(this.Context);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_folderSystem != null) _folderSystem.Dispose();
            }
        }
    }
}
