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

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    public class ScheduledFolder : RegistrationWorkflowFolder
    {
        public ScheduledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Scheduled")
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            ((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "Edit.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.OpenIconSet = new IconSet(IconScheme.Colour, "OpenItemSmall.png", "OpenItemMedium.png", "OpenItemLarge.png");
            this.IconSet = this.OpenIconSet;
        }

        protected override IList<WorklistItem> QueryItems()
        {
            return (IList<WorklistItem>)this.WorkflowService.GetWorklist("ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled");
        }

        protected override bool IsMember(WorklistItem item)
        {
            //return item.HasStatus(ActivityStatus.SC);
            return true;
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            ApplicationComponent.LaunchAsDialog(desktopWindow, new FolderOptionComponent(), "Scheduled Option");
        }
    }

    public class CheckedInFolder : RegistrationWorkflowFolder
    {
        public CheckedInFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Checked In")
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            ((SimpleActionModel) this.MenuModel).AddAction("CheckInOption", "Option", "Edit.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });
        }

        protected override IList<WorklistItem> QueryItems()
        {
            return (IList<WorklistItem>)this.WorkflowService.GetWorklist("ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn");
        }

        protected override bool IsMember(WorklistItem item)
        {
            //return item.HasStatus(ActivityStatus.IP);
            return true;
        }

        protected override bool CanAcceptDrop(WorklistItem item)
        {
            //return item.HasStatus(ActivityStatus.SC);
            return true;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistItem> items)
        {
            //DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to check in these patients?", MessageBoxActions.YesNo);
            //return (result == DialogBoxAction.Yes);

            return true;
        }

        protected override bool ProcessDrop(WorklistItem item)
        {
            //Todo: Process drop
            //service.StartProcedureStep(item.ProcedureStep);
            RequestedProcedureCheckInComponent checkInComponent = new RequestedProcedureCheckInComponent(item);
            ApplicationComponent.LaunchAsDialog(
                this.WorkflowFolderSystem.DesktopWindow, checkInComponent, String.Format("Checking in {0}", item.PatientName.ToString()));
                        
            return true;
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            ApplicationComponent.LaunchAsDialog(desktopWindow, new FolderOptionComponent(), "Check-In Option");
        }
    }

    public class InProgressFolder : RegistrationWorkflowFolder
    {
        public InProgressFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress")
        {
        }

        protected override IList<WorklistItem> QueryItems()
        {
            return (IList<WorklistItem>)this.WorkflowService.GetWorklist("ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress");
        }

        protected override bool IsMember(WorklistItem item)
        {
            //return item.HasStatus(ActivityStatus.IP);
            return true;
        }
    }

    public class CompletedFolder : RegistrationWorkflowFolder
    {
        public CompletedFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {
        }

        protected override IList<WorklistItem> QueryItems()
        {
            return (IList<WorklistItem>)this.WorkflowService.GetWorklist("ClearCanvas.Healthcare.Workflow.Registration.Worklists+Completed");
        }

        protected override bool IsMember(WorklistItem item)
        {
            //return item.HasStatus(ActivityStatus.CM);
            return true;
        }
    }

    public class CancelledFolder : RegistrationWorkflowFolder
    {
        public CancelledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled")
        {
        }

        protected override IList<WorklistItem> QueryItems()
        {
            return (IList<WorklistItem>)this.WorkflowService.GetWorklist("ClearCanvas.Healthcare.Workflow.Registration.Worklists+Cancelled");
        }

        protected override bool IsMember(WorklistItem item)
        {
            //return item.HasStatus(ActivityStatus.DC);
            return true;
        }
    }

    public class SearchFolder : RegistrationWorkflowFolder
    {
        PatientProfileSearchCriteria _searchCriteria;

        public SearchFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Search")
        {
            this.OpenIconSet = new IconSet(IconScheme.Colour, "SearchToolSmall.png", "SearchToolMedium.png", "SearchToolLarge.png");
            this.ClosedIconSet = this.OpenIconSet;
            this.IconSet = this.OpenIconSet;
        }

        public SearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set 
            { 
                _searchCriteria = value as PatientProfileSearchCriteria;
                this.Refresh();
            }
        }

        protected override IList<WorklistItem> QueryItems()
        {
            return (IList<WorklistItem>)this.WorkflowService.GetWorklist("ClearCanvas.Healthcare.Workflow.Registration.Worklists+Search", _searchCriteria);
        }

        protected override bool IsMember(WorklistItem item)
        {
            return true;
        }
    }
}
