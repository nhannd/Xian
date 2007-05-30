using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class RegistrationFolderExplorerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionOf(typeof(RegistrationFolderExplorerToolExtensionPoint))]
    public class RegistrationWorkflowFolderSystemTool : Tool<IFolderExplorerToolContext>
    {
        private RegistrationWorkflowFolderSystem _folderSystem;

        public RegistrationWorkflowFolderSystemTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _folderSystem = new RegistrationWorkflowFolderSystem(this.Context);
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
