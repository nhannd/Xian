using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
    public class TestFolder : ReportingWorkflowFolder
    {
        public TestFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Test")
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+Test";
        }
    }
}
