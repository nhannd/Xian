using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class RegistrationWorkflowFolder : WorkflowFolder<RegistrationWorklistItem>
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

        protected IRegistrationWorkflowService WorkflowService
        {
            get { return _folderSystem.WorkflowService; }
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            return false;
        }

        protected override bool ConfirmAcceptDrop(ICollection<RegistrationWorklistItem> items)
        {
            return false;
        }

        protected override bool ProcessDrop(RegistrationWorklistItem item)
        {
            return false;
        }

        protected IList<RegistrationWorklistItem> ConvertToWorkListItem(IList<RegistrationWorklistQueryResult> listQueryResult)
        {
            // Group the query results based on patient profile into a Registration Worklist item
            IDictionary<EntityRef<PatientProfile>, RegistrationWorklistItem> worklistDictionary = new Dictionary<EntityRef<PatientProfile>, RegistrationWorklistItem>();
            foreach (RegistrationWorklistQueryResult queryResult in listQueryResult)
            {
                if (worklistDictionary.ContainsKey(queryResult.PatientProfile))
                    worklistDictionary[queryResult.PatientProfile].AddQueryResults(queryResult);
                else
                    worklistDictionary[queryResult.PatientProfile] = new RegistrationWorklistItem(queryResult);
            }

            // Now add the worklist item 
            IList<RegistrationWorklistItem> worklist = new List<RegistrationWorklistItem>();
            foreach (KeyValuePair<EntityRef<PatientProfile>, RegistrationWorklistItem> kvp in worklistDictionary)
            {
                worklist.Add(kvp.Value);
            }

            return worklist;
        }

        protected IList<RegistrationWorklistItem> ConvertToWorkListItem(IList<PatientProfile> listProfile)
        {
            // Now add the worklist item 
            IList<RegistrationWorklistItem> worklist = new List<RegistrationWorklistItem>();
            foreach (PatientProfile profile in listProfile)
            {
                worklist.Add(new RegistrationWorklistItem(profile));
            }

            return worklist;
        }

    }
}