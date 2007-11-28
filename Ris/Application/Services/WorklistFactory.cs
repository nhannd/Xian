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
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System.Reflection;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services
{
    internal class WorklistFactory
    {
        private readonly Dictionary <string, Type> _worklistTypeMapping;
        private static readonly object _lock = new object();
        private static WorklistFactory _theInstance;

        private struct WorklistTokenInfo
        {
            private string _name;
            private string _description;

            public WorklistTokenInfo(string name, string description)
            {
                _name = name;
                _description = description;
            }

            public string Name
            {
                get { return _name; }
            }

            public string Description
            {
                get { return _description; }
            }
        }

        private WorklistFactory()
        {
            _worklistTypeMapping = new Dictionary<string, Type>();

            IDictionary<string, WorklistTokenInfo> tokens = GetWorklistTokens();

            WorklistExtensionPoint xp = new WorklistExtensionPoint();
            foreach (IWorklist worklist in xp.CreateExtensions())
            {
                try
                {
                    Type worklistType = worklist.GetType();
                    string worklistTypeName = GetNameParameterFromExtensionOfAttribute(worklistType);
                    
                    // TODO:  
                    WorklistTokenInfo tokenInfo = tokens[worklistTypeName];
                    _worklistTypeMapping.Add(tokenInfo.Name, worklistType);
                }
                catch (KeyNotFoundException)
                {
                    Platform.Log(LogLevel.Debug, "Worklist token not found for worklist {0}", worklist.GetType().Name);
                }
            }
        }

        private string GetNameParameterFromExtensionOfAttribute(Type worklistType)
        {
            object[] attrs = worklistType.GetCustomAttributes(typeof(ExtensionOfAttribute), false);
            return ((ExtensionOfAttribute)attrs[0]).Name;
        }


        /// <summary>
        /// Returns a list of all worklist tokens defined in installed plugins
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string, WorklistTokenInfo> GetWorklistTokens()
        {
            IDictionary<string, WorklistTokenInfo> tokens = new Dictionary<string, WorklistTokenInfo>();

            WorklistTokenExtensionPoint tokenXp = new WorklistTokenExtensionPoint();
            foreach (object o in tokenXp.CreateExtensions())
            {
                Type worklistTokenClass = o.GetType();
                foreach (FieldInfo worklistTokenField in worklistTokenClass.GetFields())
                {
                    string tokenDescription = "";

                    object[] attrs = worklistTokenField.GetCustomAttributes(typeof(WorklistTokenAttribute), false);
                    if (attrs.Length == 1)
                    {
                        tokenDescription = ((WorklistTokenAttribute)attrs[0]).Description;
                    }

                    tokens.Add(worklistTokenField.Name, new WorklistTokenInfo(worklistTokenField.Name, tokenDescription));
                }
            }

            return tokens;
        }

        public static WorklistFactory Instance
        {
            get
            {
                if(_theInstance == null)
                {
                    lock(_lock)
                    {
                        if(_theInstance == null) 
                            _theInstance = new WorklistFactory();
                    }
                }
                return _theInstance;
            }
        }

        public ICollection<string> WorklistTypes
        {
            get { return _worklistTypeMapping.Keys; }
        }

        public Worklist GetWorklist(string type)
        {
            return (Worklist)Activator.CreateInstance(GetWorklistType(type));
        }

        public Type GetWorklistType(string type)
        {
            try
            {
                return _worklistTypeMapping[type];
            }
            catch(KeyNotFoundException)
            {
                throw new RequestValidationException("Invalid worklist type");
            }
        }

        public List<string> GetWorklistClassNames(List<string> types)
        {
            return CollectionUtils.Map<string, string>(types,
                delegate(string tokens)
                {
                    return GetWorklistType(tokens).Name;
                });
        }

        public string GetWorklistType(Worklist worklist)
        {
            Type worklistType = worklist.GetType();
            foreach (KeyValuePair<string, Type> pair in _worklistTypeMapping)
            {
                if (pair.Value == worklistType)
                    return pair.Key;
            }
            return "";
        }
    }
}