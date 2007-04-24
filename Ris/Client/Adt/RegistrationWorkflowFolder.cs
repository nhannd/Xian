using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class RegistrationWorkflowFolder : WorkflowFolder<RegistrationWorklistItem>
    {
        private RegistrationWorkflowFolderSystem _folderSystem;
        private IconSet _closedIconSet;
        private IconSet _openIconSet;

        private string _worklistClassName;

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystem folderSystem, string folderName)
            : base(folderSystem, folderName, new RegistrationWorklistTable())
        {
            _folderSystem = folderSystem;

            _closedIconSet = new IconSet(IconScheme.Colour, "FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
            _openIconSet = new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
            this.IconSet = _closedIconSet;
            this.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
        }

        public string WorklistClassName
        {
            get { return _worklistClassName; }
            set { _worklistClassName = value; }
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

        protected override bool CanQuery()
        {
            return true;
        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            List<RegistrationWorklistItem> worklistItems = null;
            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    GetWorklistResponse response = service.GetWorklist(new GetWorklistRequest(this.WorklistClassName));
                    worklistItems = response.WorklistItems;
                });

            if (worklistItems == null)
                worklistItems = new List<RegistrationWorklistItem>();

            return worklistItems;
        }

        protected override int QueryCount()
        {
            int count = -1;
            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    GetWorklistResponse response = service.GetWorklist(new GetWorklistRequest(this.WorklistClassName));
                    count = response.WorklistItems.Count;
                });

            return count;
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return true;
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            return false;
        }

        protected override bool ConfirmAcceptDrop(ICollection<RegistrationWorklistItem> items)
        {
            return true;
        }

        protected override bool ProcessDrop(RegistrationWorklistItem item)
        {
            return false;
        }

        protected bool GetOperationEnablement(string operationName)
        {
            return _folderSystem.GetOperationEnablement(operationName);
        }
   }
}