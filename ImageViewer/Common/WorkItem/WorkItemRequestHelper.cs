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
        private static readonly IDictionary<WorkItemConcurrency, ReadOnlyCollection<string>> WorkItemTypesByConcurrency = InternalGetWorkItemTypesByConcurrency();

        public static readonly ReadOnlyCollection<Type> RequestRuntimeTypes = GetRequestRuntimeTypes();
        public static readonly ReadOnlyCollection<string> ActivityTypes = GetActivityTypes();

        public static ReadOnlyCollection<string> GetWorkItemTypes(this WorkItemConcurrency concurrency)
        {
            return WorkItemTypesByConcurrency[concurrency];
        }

        public static IDictionary<WorkItemConcurrency, ReadOnlyCollection<string>> GetWorkItemTypesByConcurrency()
        {
            return new Dictionary<WorkItemConcurrency, ReadOnlyCollection<string>>(WorkItemTypesByConcurrency);
        }

        private static ReadOnlyCollection<Type> GetRequestRuntimeTypes()
        {
            var types = (from p in Platform.PluginManager.Plugins
                         from t in p.Assembly.GetTypes()
                         let a = AttributeUtils.GetAttribute<WorkItemRequestAttribute>(t)
                         where (a != null)
                         select t).ToList();
            
            return types.AsReadOnly();
        }

        private static ReadOnlyCollection<string> GetActivityTypes()
        {
            // build the contract map by finding all types having a T attribute
            var types = GetRequestRuntimeTypes();
            var activityTypes = types.Select(t => Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null))
                .OfType<WorkItemRequest>()
                .Select(request => request.ActivityTypeString).ToList();

            return activityTypes.Distinct().ToList().AsReadOnly();
        }

        private static IDictionary<WorkItemConcurrency, ReadOnlyCollection<string>> InternalGetWorkItemTypesByConcurrency()
        {
            var types = GetRequestRuntimeTypes();
            var requestsByConcurrency = types.Select(t => Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null))
                .OfType<WorkItemRequest>()
                .GroupBy(w => w.ConcurrencyType);

            IDictionary<WorkItemConcurrency, ReadOnlyCollection<string>> typesByConcurrency = new Dictionary<WorkItemConcurrency, ReadOnlyCollection<string>>();
            foreach (var requestsOfConcurrency in requestsByConcurrency)
                typesByConcurrency.Add(requestsOfConcurrency.Key, requestsOfConcurrency.Select(w => w.WorkItemType).ToList().AsReadOnly());

            return typesByConcurrency;
        }
    }
}