using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{     
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class WorkItemRequestAttribute : Attribute    
    {        
    }


    public static class ActivityTypeHelper
    {
        private static List<string> _activityTypes;
        private static readonly object SyncLock = new Object();

        public static IEnumerable<string> GetActivityTypeList()
        {
            lock (SyncLock)
            {
                if (_activityTypes == null)
                {
                    // build the contract map by finding all types having a T attribute
                    var types = (from p in Platform.PluginManager.Plugins
                                 from t in p.Assembly.GetTypes()
                                 let a = AttributeUtils.GetAttribute<WorkItemRequestAttribute>(t)
                                 where (a != null)
                                 select t).ToList();

                    _activityTypes = new List<string>();

                    foreach (Type t in types)
                    {
                        var dataObject = Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                        var request = dataObject as WorkItemRequest;
                        if (request != null)
                            _activityTypes.Add(request.ActivityTypeString);
                    }
                }

                return _activityTypes;
            }
        }      
    }
}
