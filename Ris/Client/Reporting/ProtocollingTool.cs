#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Protocol", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [LabelValueObserver("apply", "Label", "LabelChanged")]
    [ExtensionOf(typeof(ReportingProtocolWorkflowItemToolExtensionPoint))]
    public class ProtocollingTool : Tool<IReportingWorkflowItemToolContext>
    {
        public void Apply()
        {
            try
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);
                OpenItem(item);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }

        private void OpenItem(ReportingWorklistItem item)
        {
            if (item == null)
                return;

            Workspace workspace = DocumentManager.Get<ProtocollingComponentDocument>(item.OrderRef);
            if (workspace == null)
            {
                ProtocollingComponentDocument protocollingComponentDocument = new ProtocollingComponentDocument(item, GetMode(item), this.Context);
                protocollingComponentDocument.Open();
            }
            else
            {
                workspace.Activate();
            }
        }

        public bool Enabled
        {
            get
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);
                return item != null && item.ProcedureStepName == "Protocol";
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

        public string Label
        {
            get
            {
                ReportingWorklistItem item = CollectionUtils.FirstElement<ReportingWorklistItem>(this.Context.SelectedItems);
                return GetLabel(item);
            }
        }

        public virtual event EventHandler LabelChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

        private ProtocollingComponentMode GetMode(ReportingWorklistItem item)
        {
            if (item == null)
                return ProtocollingComponentMode.Review;

            switch(item.ActivityStatus.Code)
            {
                case "SC":
                    return ProtocollingComponentMode.Assign;
                case "IP":
                    return ProtocollingComponentMode.Edit;
                default:
                    return ProtocollingComponentMode.Review;
            }
        }

        private string GetLabel(ReportingWorklistItem item)
        {
            if (item == null)
                return "Review Protocol";

            switch (item.ActivityStatus.Code)
            {
                case "SC":
                    return "Assign Protocol";
                case "IP":
                    return "Edit Protocol";
                default:
                    return "Review Protocol";
            }
        }
    }
}
