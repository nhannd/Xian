using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    internal class WorkItemChangedEventWrapper
    {
        private event EventHandler<WorkItemChangedEventArgs> _changed;
        private volatile bool _isSubscribedToService;

        public WorkItemChangedEventWrapper(WorkItemTypeEnum? workItemType)
        {
            WorkItemType = workItemType;
        }

        public WorkItemTypeEnum? WorkItemType { get; private set; }

        public bool HasChangedDelegates { get { return _changed != null; } }

        public bool IsSubscribedToService
        {
            get { return _isSubscribedToService; }
            set { _isSubscribedToService = value; }
        }

        public bool ShouldSubscribeToService { get { return HasChangedDelegates && !IsSubscribedToService; } }
        public bool ShouldUnsubscribeFromService { get { return !HasChangedDelegates && IsSubscribedToService; } }

        public bool IsActive { get { return HasChangedDelegates || IsSubscribedToService; } }

        public event EventHandler<WorkItemChangedEventArgs> Changed
        {
            add { _changed += value; }
            remove { _changed -= value; }
        }

        public Delegate[] GetChangedDelegates()
        {
            //Don't return any if we're not actually subscribed on the service.
            //if (!IsSubscribedToService)
            //    return new Delegate[0];

            return _changed != null ? _changed.GetInvocationList() : new Delegate[0];
        }
    }
    
    internal class WorkItemChangedEventWrappers
    {
        private Dictionary<WorkItemTypeEnum, WorkItemChangedEventWrapper> _typeWrappers;
        private WorkItemChangedEventWrapper _allTypesWrapper;

        public WorkItemChangedEventWrapper AllTypesWrapper
        {
            get
            {
                if (_allTypesWrapper == null)
                    _allTypesWrapper = new WorkItemChangedEventWrapper(null);
                return _allTypesWrapper;
            }
        }

        public IDictionary<WorkItemTypeEnum, WorkItemChangedEventWrapper> TypeWrappers
        {
            get
            {
                if (_typeWrappers == null)
                    _typeWrappers = new Dictionary<WorkItemTypeEnum, WorkItemChangedEventWrapper>();
                return _typeWrappers;
            }
        }

        public WorkItemChangedEventWrapper this[WorkItemTypeEnum? workItemType]
        {
            get
            {
                if (!workItemType.HasValue)
                    return AllTypesWrapper;

                WorkItemChangedEventWrapper wrapper;
                if (!TypeWrappers.TryGetValue(workItemType.Value, out wrapper))
                {
                    wrapper = new WorkItemChangedEventWrapper(workItemType);
                    TypeWrappers[workItemType.Value] = wrapper;
                }

                return wrapper;
            }
        }

        public IList<WorkItemChangedEventWrapper> GetActiveWrappers()
        {
            var wrappers = new List<WorkItemChangedEventWrapper>();
            if (_allTypesWrapper != null && _allTypesWrapper.IsActive)
                wrappers.Add(_allTypesWrapper);

            foreach (var wrapper in TypeWrappers.Values.Where(w => w.IsActive))
                wrappers.Add(wrapper);

            return wrappers;
        }

        public IList<Delegate> GetChangedDelegates(WorkItemTypeEnum workItemType)
        {
            return GetActiveWrappers(workItemType).SelectMany(w => w.GetChangedDelegates()).ToList();
        }

        public IList<WorkItemChangedEventWrapper> GetActiveWrappers(WorkItemTypeEnum workItemType)
        {
            return GetActiveWrappers().Where(w => !w.WorkItemType.HasValue || w.WorkItemType == workItemType).ToList();
        }

        public IList<WorkItemChangedEventWrapper> GetWrappersToSubscribeToService()
        {
            return GetActiveWrappers().Where(w => w.ShouldSubscribeToService).ToList();
        }

        public IList<WorkItemChangedEventWrapper> GetWrappersToUnsubscribeFromService()
        {
            return GetActiveWrappers().Where(w => w.ShouldUnsubscribeFromService).ToList();
        }
    }
}