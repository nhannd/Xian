using System;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    internal class FolderFactory
    {
        private readonly Dictionary<string, Type> _worklistTypeMapping;
        private static readonly object _lock = new object();
        private static FolderFactory _theInstance;

        private FolderFactory()
        {
            // TODO: populate dictionary from worklist classes themselves, not hard-coded
            _worklistTypeMapping = new Dictionary<string, Type>();
            _worklistTypeMapping.Add("Registration - Checked In", typeof(Folders.CheckedInFolder));
            _worklistTypeMapping.Add("Registration - In Progress", typeof(Folders.InProgressFolder));
            _worklistTypeMapping.Add("Registration - Scheduled", typeof(Folders.ScheduledFolder));
            _worklistTypeMapping.Add("Registration - Cancelled", typeof(Folders.CancelledFolder));
            _worklistTypeMapping.Add("Registration - Completed", typeof(Folders.CompletedFolder));
            _worklistTypeMapping.Add("Technologist - Checked In", typeof(Folders.CheckedInTechnologistWorkflowFolder));
            _worklistTypeMapping.Add("Technologist - In Progress", typeof(Folders.InProgressTechnologistWorkflowFolder));
            _worklistTypeMapping.Add("Technologist - Scheduled", typeof(Folders.ScheduledTechnologistWorkflowFolder));
            _worklistTypeMapping.Add("Technologist - Cancelled", typeof(Folders.CancelledTechnologistWorkflowFolder));
            _worklistTypeMapping.Add("Technologist - Completed", typeof(Folders.CompletedTechnologistWorkflowFolder));
        }

        public static FolderFactory Instance
        {
            get
            {
                if (_theInstance == null)
                {
                    lock (_lock)
                    {
                        if (_theInstance == null)
                            _theInstance = new FolderFactory();
                    }
                }
                return _theInstance;
            }
        }

        public WorkflowFolder<RegistrationWorklistItem> GetFolder(string type, RegistrationWorkflowFolderSystem folderSystem, WorklistSummary worklistSummary)
        {
            Type foundType = GetWorklistType(type);
            //return foundType != null
            //    ? (WorkflowFolder<ModalityWorklistItem>)Activator.CreateInstance(foundType, folderSystem, worklistSummary.Name, worklistSummary.EntityRef)
            //    : null;
            try
            {
                return (WorkflowFolder<RegistrationWorklistItem>)Activator.CreateInstance(foundType, folderSystem, worklistSummary.DisplayName, worklistSummary.EntityRef);
            }
            catch (MissingMethodException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public WorkflowFolder<ModalityWorklistItem> GetFolder(string type, TechnologistWorkflowFolderSystem folderSystem, WorklistSummary worklistSummary)
        {
            Type foundType = GetWorklistType(type);
            //return foundType != null
            //    ? (WorkflowFolder<ModalityWorklistItem>)Activator.CreateInstance(foundType, folderSystem, worklistSummary.Name, worklistSummary.EntityRef)
            //    : null;
            try
            {
                return (WorkflowFolder<ModalityWorklistItem>)Activator.CreateInstance(foundType, folderSystem, worklistSummary.DisplayName, worklistSummary.EntityRef);
            }
            catch (MissingMethodException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public Type GetWorklistType(string type)
        {
            try
            {
                return _worklistTypeMapping[type];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}