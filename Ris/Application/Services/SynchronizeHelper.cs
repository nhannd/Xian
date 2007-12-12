using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class SynchronizeHelper<TDomainItem, TSourceItem>
        where TDomainItem : DomainObject
        where TSourceItem : DataContractBase
    {
        public delegate bool CompareItemsDelegate(TDomainItem domainItem, TSourceItem sourceItem);
        public delegate TDomainItem CreateDomainItemDelegate(TSourceItem item);
        public delegate void UpdateDomainItemDelegate(TDomainItem domainItem, TSourceItem sourceItem);

        private readonly CompareItemsDelegate _compareItemsCallback;
        private readonly CreateDomainItemDelegate _createDomainItemCallback;
        private readonly UpdateDomainItemDelegate _updateDomainItemCallback;

        /// <summary>
        /// Protected constructor for subclasses.
        /// </summary>
        protected SynchronizeHelper()
        {
            
        }

        /// <summary>
        /// Public constructor allows direct use of this class without the need to create a subclass.
        /// </summary>
        /// <param name="compareItemsCallback"></param>
        /// <param name="createDomainItemCallback"></param>
        /// <param name="updateDomainItemCallback"></param>
        public SynchronizeHelper(
            CompareItemsDelegate compareItemsCallback,
            CreateDomainItemDelegate createDomainItemCallback,
            UpdateDomainItemDelegate updateDomainItemCallback)
        {
            _compareItemsCallback = compareItemsCallback;
            _createDomainItemCallback = createDomainItemCallback;
            _updateDomainItemCallback = updateDomainItemCallback;
        }

        /// <summary>
        /// Synchronize the domainList using the sourceList, add/remove/update all items in the domainList
        /// </summary>
        /// <param name="domainList"></param>
        /// <param name="sourceList"></param>
        public void Synchronize(IList<TDomainItem> domainList, IList<TSourceItem> sourceList)
        {
            IList<TDomainItem> unProcessed = new List<TDomainItem>(domainList);

            CollectionUtils.ForEach(sourceList,
                delegate(TSourceItem sourceItem)
                {
                    // Find a domain item that matches the source item
                    TDomainItem foundDomainItem = CollectionUtils.SelectFirst(domainList,
                        delegate(TDomainItem domainItem) { return CompareItems(domainItem, sourceItem); });

                    if (foundDomainItem == null)
                    {
                        // Add a new domain item
                        domainList.Add(CreateDomainItem(sourceItem));
                    }
                    else
                    {
                        // Update the existing attachment
                        UpdateDomainItem(foundDomainItem, sourceItem);

                        // and remove from un-processed list
                        unProcessed.Remove(foundDomainItem);
                    }
                });

            // Remove any unprocessed items from the domain list
            if (unProcessed.Count > 0)
            {
                CollectionUtils.ForEach(unProcessed,
                    delegate(TDomainItem domainItem)
                    {
                        domainList.Remove(domainItem);
                    });
            }
        }

        protected virtual bool CompareItems(TDomainItem domainItem, TSourceItem sourceItem)
        {
            if (_compareItemsCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");

            return _compareItemsCallback(domainItem, sourceItem);
        }

        protected virtual TDomainItem CreateDomainItem(TSourceItem sourceItem)
        {
            if (_createDomainItemCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");

            return _createDomainItemCallback(sourceItem);
        }

        protected virtual void UpdateDomainItem(TDomainItem domainItem, TSourceItem sourceItem)
        {
            if (_updateDomainItemCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");

            _updateDomainItemCallback(domainItem, sourceItem);
        }
    }
}
