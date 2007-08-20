using System;
using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System.Reflection;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;

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