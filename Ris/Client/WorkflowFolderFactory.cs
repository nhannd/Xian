using System;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class WorkflowFolderFactory
    {
        private readonly Dictionary<string, Type> _worklistTypeMapping;
        private static readonly object _lock = new object();
        private static WorkflowFolderFactory _theInstance;

        private WorkflowFolderFactory()
        {
            _worklistTypeMapping = new Dictionary<string, Type>();

            WorkflowFolderExtensionPoint xp = new WorkflowFolderExtensionPoint();
            foreach (IFolder folder in xp.CreateExtensions())
            {
                Type folderType = folder.GetType();
                string worklistType = GetFolderForWorklistTypeAttributeValue(folderType);
                if(string.IsNullOrEmpty(worklistType) == false)
                {
                    _worklistTypeMapping.Add(worklistType, folderType);
                }
            }
        }

        private string GetFolderForWorklistTypeAttributeValue(Type type)
        {
            foreach (object o in type.GetCustomAttributes(false))
            {
                FolderForWorklistTypeAttribute attrib = o as FolderForWorklistTypeAttribute;
                if(attrib != null)
                {
                    return attrib.WorklistType;
                }
            }
            return null;
        }

        public static WorkflowFolderFactory Instance
        {
            get
            {
                if (_theInstance == null)
                {
                    lock (_lock)
                    {
                        if (_theInstance == null)
                            _theInstance = new WorkflowFolderFactory();
                    }
                }
                return _theInstance;
            }
        }

        public WorkflowFolder<TItem> GetFolder<TItem>(string type, WorkflowFolderSystem<TItem> folderSystem, WorklistSummary worklistSummary)
        {
            Type foundType = GetWorklistType(type);
            return (WorkflowFolder<TItem>)Activator.CreateInstance(foundType, folderSystem, worklistSummary.DisplayName, worklistSummary.EntityRef);
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