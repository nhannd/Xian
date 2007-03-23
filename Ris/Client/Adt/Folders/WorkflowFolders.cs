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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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

            this.WorklistClassName = "Scheduled";
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

            this.WorklistClassName = "CheckIn";
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            return item.WorklistClassName == "Scheduled";
        }

        protected override bool ProcessDrop(RegistrationWorklistItem item)
        {
            RequestedProcedureCheckInComponent checkInComponent = new RequestedProcedureCheckInComponent(item);
            ApplicationComponent.LaunchAsDialog(
                this.WorkflowFolderSystem.DesktopWindow, checkInComponent, String.Format("Checking in {0}", Format.Custom(item.Name)));
                        
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
            this.WorklistClassName = "InProgress";
        }
    }

    public class CompletedFolder : RegistrationWorkflowFolder
    {
        public CompletedFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {
            this.WorklistClassName = "Completed";
        }
    }

    public class CancelledFolder : RegistrationWorkflowFolder
    {
        public CancelledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled")
        {
            this.WorklistClassName = "Cancelled";
        }
    }

    public class SearchFolder : RegistrationWorkflowFolder
    {
        public SearchFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Search")
        {
            this.OpenIconSet = new IconSet(IconScheme.Colour, "SearchToolSmall.png", "SearchToolMedium.png", "SearchToolLarge.png");
            this.ClosedIconSet = this.OpenIconSet;
            this.IconSet = this.OpenIconSet;

            this.WorklistClassName = "Search";
        }
    }
}
