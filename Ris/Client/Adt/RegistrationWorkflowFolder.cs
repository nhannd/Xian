using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class RegistrationWorkflowFolder : WorkflowFolder<WorklistItem>
    {
        private RegistrationWorkflowFolderSystem _folderSystem;
        private IconSet _closedIconSet;
        private IconSet _openIconSet;

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystem folderSystem, string folderName)
            : base(folderSystem, folderName, new RegistrationWorklistTable())
        {
            _folderSystem = folderSystem;

            _closedIconSet = new IconSet(IconScheme.Colour, "FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
            _openIconSet = new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
            this.IconSet = _closedIconSet;
            this.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
        }

        public IconSet ClosedIconSet
        {
            get { return _closedIconSet; }
            set { _closedIconSet = value; }
        }

        public IconSet OpenIconSet
        {
            get { return _openIconSet; }
            set { _openIconSet = value; }
        }

        public override void OpenFolder()
        {
            if (_openIconSet != null)
                this.IconSet = _openIconSet;

            base.OpenFolder();
        }

        public override void CloseFolder()
        {
            if (_closedIconSet != null)
                this.IconSet = _closedIconSet;

            base.CloseFolder();
        }

        protected IWorklistService WorkflowService
        {
            get { return _folderSystem.WorkflowService; }
        }

        protected override bool CanAcceptDrop(WorklistItem item)
        {
            return false;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistItem> items)
        {
            return false;
        }

        protected override bool ProcessDrop(WorklistItem item)
        {
            return false;
        }
    }
}