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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Client.Modality.Folders
{
    public class ScheduledFolder : ModalityWorkflowFolder
    {
        public ScheduledFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Scheduled")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.SC);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.SC;
        }
    }

    public class InProgressFolder : ModalityWorkflowFolder
    {
        public InProgressFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.IP);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.IP;
        }

        protected override bool CanAcceptDrop(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.SC;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistQueryResult> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to start these procedures?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(WorklistQueryResult item)
        {
            IModalityWorkflowService service = ApplicationContext.GetService<IModalityWorkflowService>();
            //service.StartProcedureStep(item.ProcedureStep);
            return true;
        }
    }

    public class CompletedFolder : ModalityWorkflowFolder
    {
        public CompletedFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.CM);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.CM;
        }

        protected override bool CanAcceptDrop(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.IP;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistQueryResult> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to complete these procedures?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(WorklistQueryResult item)
        {
            IModalityWorkflowService service = ApplicationContext.GetService<IModalityWorkflowService>();
            //service.CompleteProcedureStep(item.ProcedureStep);
            return true;
        }
    }

    public class CancelledFolder : ModalityWorkflowFolder
    {
        public CancelledFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.DC);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.DC;
        }

        protected override bool CanAcceptDrop(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.IP || item.Status == ActivityStatus.SC;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistQueryResult> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to cancel these procedures?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(WorklistQueryResult item)
        {
            IModalityWorkflowService service = ApplicationContext.GetService<IModalityWorkflowService>();
            //service.CancelProcedureStep(item.ProcedureStep);
            return true;
        }
    }
}
