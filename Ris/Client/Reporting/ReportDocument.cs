using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportDocument : Document
    {
        private ReportingWorklistItem _worklistItem;
        private string _reportContent;
        private IEnumerable _reportFolders;

        public ReportDocument(ReportingWorklistItem item, string reportContent, IEnumerable folders, IDesktopWindow window)
            : base(item.ProcedureStepRef, window)
        {
            _worklistItem = item;
            _reportContent = reportContent;
            _reportFolders = folders;
        }

        public ReportingWorklistItem WorklistItem
        {
            get { return _worklistItem; }
        }

        public string ReportContent
        {
            get { return _reportContent; }
        }

        public IEnumerable Folders
        {
            get { return _reportFolders; }
        }

        protected override string GetTitle()
        {
            return String.Format("Report - {0} ({1})", PersonNameFormat.Format(_worklistItem.PersonNameDetail), _worklistItem.RequestedProcedureName);
        }

        protected override IApplicationComponent GetComponent()
        {
            // Create tab and tab groups
            TabComponentContainer tabContainer1 = new TabComponentContainer();
            tabContainer1.Pages.Add(new TabPage("Prior Report", new PriorReportComponent()));
            tabContainer1.Pages.Add(new TabPage("Exam", new PriorReportComponent()));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer1, 1.0f));

            ReportContentEditorComponent reportContentEditor = new ReportContentEditorComponent(_worklistItem, _reportContent, _reportFolders);
            reportContentEditor.CloseComponentRequest += new EventHandler(reportContentEditor_CloseComponentRequest);

            // Construct the Patient Biography page
            return new SplitComponentContainer(
                new SplitPane("", reportContentEditor, 0.5f),
                new SplitPane("", tabGroupContainer, 0.5f),
                SplitOrientation.Vertical);
        }

        void reportContentEditor_CloseComponentRequest(object sender, EventArgs e)
        {
            this.Close();
        }
    }    
}
