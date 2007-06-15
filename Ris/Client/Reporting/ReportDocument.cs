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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportDocument : Document
    {
        private ReportingWorklistItem _worklistItem;
        private IEnumerable _reportFolders;
        private bool _readOnly;

        public ReportDocument(ReportingWorklistItem item, IEnumerable folders, bool readOnly, IDesktopWindow window)
            : base(item.ProcedureStepRef, window)
        {
            _worklistItem = item;
            _reportFolders = folders;
            _readOnly = readOnly;
        }

        protected override string GetTitle()
        {
            return String.Format("Report - {0} ({1})", PersonNameFormat.Format(_worklistItem.PersonNameDetail), _worklistItem.RequestedProcedureName);
        }

        protected override IApplicationComponent GetComponent()
        {
            // Create tab and tab groups
            TabComponentContainer tabContainer1 = new TabComponentContainer();
            tabContainer1.Pages.Add(new TabPage("Prior Report", new PriorReportComponent(_worklistItem)));
            tabContainer1.Pages.Add(new TabPage("Exam", new PriorReportComponent(_worklistItem)));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer1, 1.0f));

            ReportContentEditorComponent reportContentEditor = new ReportContentEditorComponent(_worklistItem, _readOnly);
            reportContentEditor.VerifyEvent += new EventHandler(reportContentEditor_VerifyEvent);
            reportContentEditor.SendToVerifyEvent += new EventHandler(reportContentEditor_SendToVerifyEvent);
            reportContentEditor.SendToTranscriptionEvent += new EventHandler(reportContentEditor_SendToTranscriptionEvent);
            reportContentEditor.CloseComponentEvent += new EventHandler(reportContentEditor_CloseComponentEvent);

            // Construct the Patient Biography page
            return new SplitComponentContainer(
                new SplitPane("", reportContentEditor, 0.5f),
                new SplitPane("", tabGroupContainer, 0.5f),
                SplitOrientation.Vertical);
        }

        void reportContentEditor_VerifyEvent(object sender, EventArgs e)
        {
            IFolder myVerifiedFolder = CollectionUtils.SelectFirst<IFolder>(_reportFolders,
                delegate(IFolder folder) 
                { 
                    return folder is Folders.VerifiedFolder;
                });

            if (myVerifiedFolder != null)
            {
                if (myVerifiedFolder.IsOpen)
                    myVerifiedFolder.Refresh();
                else
                    myVerifiedFolder.RefreshCount();
            }

            this.Close();
        }

        void reportContentEditor_SendToVerifyEvent(object sender, EventArgs e)
        {
            IFolder myVerificationFolder = CollectionUtils.SelectFirst<IFolder>(_reportFolders,
                delegate(IFolder folder)
                {
                    return folder is Folders.ToBeVerifiedFolder;
                });

            if (myVerificationFolder != null)
            {
                if (myVerificationFolder.IsOpen)
                    myVerificationFolder.Refresh();
                else
                    myVerificationFolder.RefreshCount();
            }

            this.Close();
        }

        void reportContentEditor_SendToTranscriptionEvent(object sender, EventArgs e)
        {
            IFolder myTranscriptionFolder = CollectionUtils.SelectFirst<IFolder>(_reportFolders,
                delegate(IFolder folder)
                {
                    return folder is Folders.InTranscriptionFolder;
                });

            if (myTranscriptionFolder != null)
            {
                if (myTranscriptionFolder.IsOpen)
                    myTranscriptionFolder.Refresh();
                else
                    myTranscriptionFolder.RefreshCount();
            }

            this.Close();
        }

        void reportContentEditor_CloseComponentEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }    
}
