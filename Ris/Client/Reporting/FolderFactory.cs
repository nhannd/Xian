using System;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
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
            _worklistTypeMapping.Add("Reporting - To Be Reported", typeof(Folders.ToBeReportedFolder));
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

        public WorkflowFolder<ReportingWorklistItem> GetFolder(string type, ReportingWorkflowFolderSystem folderSystem, WorklistSummary worklistSummary)
        {
            try
            {
                Type foundType = GetWorklistType(type);
                return (WorkflowFolder<ReportingWorklistItem>)Activator.CreateInstance(foundType, folderSystem, worklistSummary.DisplayName, worklistSummary.EntityRef);
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