using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Reporting
{
    //[ExtensionOf(typeof(FolderExplorerToolExtensionPoint))]
    public class ReportingFolderSystemTool : Tool<IFolderExplorerToolContext>
    {
        private IReportingWorkflowService _workflowService;
        private ReportingWorklistTable _itemsTable;

        public ReportingFolderSystemTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _workflowService = ApplicationContext.GetService<IReportingWorkflowService>();

            _itemsTable = new ReportingWorklistTable(_workflowService.GetOrderPriorityEnumTable());

            //this.Context.AddFolder(new Folders.ScheduledInterpretationFolder(_workflowService, _itemsTable));
        }
    }
}
