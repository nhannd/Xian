using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class TechnologistFolderExplorerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionOf(typeof(TechnologistFolderExplorerToolExtensionPoint))]
    class TechnologistWorkflowFolderSystemTool : Tool<IFolderExplorerToolContext>
    {
        private TechnologistWorkflowFolderSystem _folderSystem;

        public TechnologistWorkflowFolderSystemTool()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            _folderSystem = new TechnologistWorkflowFolderSystem(this.Context);
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
