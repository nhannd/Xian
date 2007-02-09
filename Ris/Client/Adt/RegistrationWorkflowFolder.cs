using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class RegistrationWorkflowFolder : WorkflowFolder<RegistrationWorklistItem>
    {
        private RegistrationWorkflowFolderSystem _folderSystem;

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystem folderSystem, string folderName)
            : base(folderSystem, folderName, new RegistrationWorklistTable())
        {
            _folderSystem = folderSystem;
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
    }
}