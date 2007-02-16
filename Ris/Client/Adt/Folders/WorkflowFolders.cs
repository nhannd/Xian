using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;
using ClearCanvas.Workflow;

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
        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(DateTime.Today, DateTime.Today.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.SC);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.SC);
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

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            
            // TODO: We don't have a Check-in status yet... so we can't query the Activity Status
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            //criteria.State.EqualTo(ActivityStatus.IP);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.IP);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.SC);
        }

        protected override bool ConfirmAcceptDrop(ICollection<RegistrationWorklistItem> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to check in these patients?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(RegistrationWorklistItem item)
        {
            IRegistrationWorkflowService service = ApplicationContext.GetService<IRegistrationWorkflowService>();
            //service.StartProcedureStep(item.ProcedureStep);
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

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.IP);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.IP);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            // For Registration, we do not allow Clerks to change status to InProgress
            //return item.HasStatus(ActivityStatus.SC);
            return false;
        }
    }

    public class CompletedFolder : RegistrationWorkflowFolder
    {
        public CompletedFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {

        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.CM);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.CM);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            // For Registration, we do not allow Clerks to change status to Completed
            //return item.HasStatus(ActivityStatus.IP);
            return false;
        }
    }

    public class CancelledFolder : RegistrationWorkflowFolder
    {
        public CancelledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled")
        {

        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.DC);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.DC);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            // For Registration, we do not allow Clerks to change status to Cancel
            //return item.HasStatus(ActivityStatus.SC);
            return false;
        }
    }

    public class SearchFolder : RegistrationWorkflowFolder
    {
        PatientProfileSearchCriteria _searchCriteria;

        public SearchFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Search")
        {
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

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            if (_searchCriteria == null)
                return new List<RegistrationWorklistItem>();

            IAdtService service = ApplicationContext.GetService<IAdtService>();
            return ConvertToWorkListItem(service.ListPatientProfiles(_searchCriteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return true;
        }
    }
}
