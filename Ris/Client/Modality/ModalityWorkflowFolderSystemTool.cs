using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Modality
{
    [ExtensionOf(typeof(FolderExplorerToolExtensionPoint))]
    public class ModalityWorkflowFolderSystemTool : Tool<IFolderExplorerToolContext>
    {
        private ModalityWorkflowFolderSystem _folderSystem;

        public ModalityWorkflowFolderSystemTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _folderSystem = new ModalityWorkflowFolderSystem(this.Context);
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
