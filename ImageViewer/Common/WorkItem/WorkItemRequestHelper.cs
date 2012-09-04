using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public static class WorkItemRequestHelper
    {
        private static readonly Type[] _requestRuntimeTypes = InternalGetRequestRuntimeTypes();
        private static readonly string[] _activityTypes = InternalGetActivityTypes();
        private static readonly IDictionary<WorkItemConcurrency, string[]> _workItemTypesByConcurrency = InternalGetWorkItemTypesByConcurrency();

        public static List<Type> GetWorkItemRequestRuntimeTypes()
        {
            return new List<Type>(_requestRuntimeTypes);
        }
        
        public static List<string> GetWorkItemTypes(this WorkItemConcurrency concurrency)
        {
            return new List<string>(_workItemTypesByConcurrency[concurrency]);
        }

        public static List<string> GetActivityTypes()
        {
            return new List<string>(_activityTypes);
        }

        public static Dictionary<WorkItemConcurrency, List<string>> GetWorkItemTypesByConcurrency()
        {
            return _workItemTypesByConcurrency.ToDictionary(k => k.Key, v => v.Value.ToList());
        }

        private static Type[] InternalGetRequestRuntimeTypes()
        {
            var types = (from p in Platform.PluginManager.Plugins
                         from t in p.Assembly.GetTypes()
                         let a = AttributeUtils.GetAttribute<WorkItemRequestAttribute>(t)
                         where (a != null)
                         select t);

            return types.ToArray();
        }

        private static string[] InternalGetActivityTypes()
        {
            // build the contract map by finding all types having a T attribute
            var types = InternalGetRequestRuntimeTypes();
            var activityTypes = types.Select(t => Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null))
                .OfType<WorkItemRequest>()
                .Select(request => request.ActivityTypeString).ToList();

            return activityTypes.Distinct().ToArray();
        }

        private static IDictionary<WorkItemConcurrency, string[]> InternalGetWorkItemTypesByConcurrency()
        {
            var types = InternalGetRequestRuntimeTypes();
            var requestsByConcurrency = types.Select(t => Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null))
                .OfType<WorkItemRequest>()
                .GroupBy(w => w.ConcurrencyType);

            return requestsByConcurrency.ToDictionary(k => k.Key, v => v.Select(w => w.WorkItemType).ToArray());
        }
    }
}