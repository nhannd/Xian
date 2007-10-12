#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
                if (string.IsNullOrEmpty(worklistType) == false)
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
            return (WorkflowFolder<TItem>)Activator.CreateInstance(foundType, folderSystem, worklistSummary.DisplayName, worklistSummary.Description, worklistSummary.EntityRef);
        }

        public Type GetWorklistType(string type)
        {
            Type worklistType;

            if(_worklistTypeMapping.TryGetValue(type, out worklistType) == false)
            {
                worklistType = null;
            }

            return worklistType;
        }
    }
}