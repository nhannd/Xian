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
        private EntityRef _reportingStepRef;
        private PersonNameDetail _patientName;
        private IEnumerable _reportFolders;

        public ReportDocument(EntityRef reportingStepRef, PersonNameDetail patientName, IEnumerable folders, IDesktopWindow window)
            : base(reportingStepRef, window)
        {
            _reportingStepRef = reportingStepRef;
            _patientName = patientName;
            _reportFolders = folders;
        }

        protected override string GetTitle()
        {
            return String.Format("Report - {0}", PersonNameFormat.Format(_patientName));
        }

        protected override IApplicationComponent GetComponent()
        {
            // Create tab and tab groups
            TabComponentContainer tabContainer1 = new TabComponentContainer();
            tabContainer1.Pages.Add(new TabPage("Prior Report", new PriorReportComponent(_reportingStepRef)));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer1, 1.0f));

            ReportEditorComponent reportEditor = new ReportEditorComponent(_reportingStepRef);
            reportEditor.VerifyEvent += new EventHandler(reportEditor_VerifyEvent);
            reportEditor.SendToVerifyEvent += new EventHandler(reportEditor_SendToVerifyEvent);
            reportEditor.SendToTranscriptionEvent += new EventHandler(reportEditor_SendToTranscriptionEvent);
            reportEditor.SaveEvent += new EventHandler(reportEditor_SaveEvent);
            reportEditor.CloseComponentEvent += new EventHandler(reportEditor_CloseComponentEvent);

            // Construct the Patient Biography page
            return new SplitComponentContainer(
                new SplitPane("", reportEditor, 0.5f),
                new SplitPane("", tabGroupContainer, 0.5f),
                SplitOrientation.Vertical);
        }

        void reportEditor_VerifyEvent(object sender, EventArgs e)
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

        void reportEditor_SendToVerifyEvent(object sender, EventArgs e)
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

        void reportEditor_SendToTranscriptionEvent(object sender, EventArgs e)
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

        void reportEditor_SaveEvent(object sender, EventArgs e)
        {
            IFolder toBeReportedFolder = CollectionUtils.SelectFirst<IFolder>(_reportFolders,
                delegate(IFolder folder)
                {
                    return folder is Folders.ToBeReportedFolder;
                });

            if (toBeReportedFolder != null && toBeReportedFolder.IsOpen)
            {
                IFolder draftFolder = CollectionUtils.SelectFirst<IFolder>(_reportFolders,
                    delegate(IFolder folder)
                    {
                        return folder is Folders.DraftFolder;
                    });

                draftFolder.RefreshCount();
            }
           
            this.Close();
        }

        void reportEditor_CloseComponentEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }    
}
