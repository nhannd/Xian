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
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled";
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    public class CheckedInFolder : RegistrationWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<RegistrationWorklistItem>>
        {
        }

        public CheckedInFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Checked In", new DropHandlerExtensionPoint())
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn";
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    public class InProgressFolder : RegistrationWorkflowFolder
    {
        public InProgressFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress")
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress";
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    public class CompletedFolder : RegistrationWorkflowFolder
    {
        public CompletedFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Completed";
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    public class CancelledFolder : RegistrationWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<RegistrationWorklistItem>>
        {
        }

        public CancelledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled", new DropHandlerExtensionPoint())
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Cancelled";
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    public class SearchFolder : RegistrationWorkflowFolder
    {
        private PatientProfileSearchData _searchCriteria;

        public SearchFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Search")
        {
			this.OpenIconSet = new IconSet(IconScheme.Colour, "SearchFolderOpenSmall.png", "SearchFolderOpenMedium.png", "SearchFolderOpenLarge.png");
			this.ClosedIconSet = new IconSet(IconScheme.Colour, "SearchFolderClosedSmall.png", "SearchFolderClosedMedium.png", "SearchFolderClosedLarge.png");
            if (this.IsOpen)
                this.IconSet = this.OpenIconSet;
            else
                this.IconSet = this.ClosedIconSet;

            this.RefreshTime = 0;
            //this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Search";
        }

        public PatientProfileSearchData SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                this.Refresh();
            }
        }

        protected override bool CanQuery()
        {
            if (this.SearchCriteria != null)
                return true;

            return false;
        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            List<RegistrationWorklistItem> worklistItems = null;
            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    SearchPatientResponse response = service.SearchPatient(new SearchPatientRequest(this.SearchCriteria));
                    worklistItems = response.WorklistItems;
                });

            if (worklistItems == null)
                worklistItems = new List<RegistrationWorklistItem>();

            return worklistItems;
        }

        protected override int QueryCount()
        {
            return this.ItemCount;
        }

    }
}
