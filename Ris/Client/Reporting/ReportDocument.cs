#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

        public ReportDocument(ReportingWorklistItem worklistItem, IEnumerable folders, IDesktopWindow window)
            : base(worklistItem.ProcedureStepRef, window)
        {
            _worklistItem = worklistItem;
            _reportFolders = folders;
        }

        protected override string GetTitle()
        {
            return String.Format("Report - {0}", PersonNameFormat.Format(_worklistItem.PatientName));
        }

        protected override IApplicationComponent GetComponent()
        {
            return new ReportingComponent(_worklistItem);
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
