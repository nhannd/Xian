#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

        protected bool _allowUpdate = false;
        protected bool _allowRemove = false;

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
        /// <param name="allowRemove"></param>
        public SynchronizeHelper(
            CompareItemsDelegate compareItemsCallback,
            CreateDomainItemDelegate createDomainItemCallback,
            UpdateDomainItemDelegate updateDomainItemCallback,
            bool allowRemove)
        {
            _compareItemsCallback = compareItemsCallback;
            _createDomainItemCallback = createDomainItemCallback;
            _updateDomainItemCallback = updateDomainItemCallback;

            _allowUpdate = _updateDomainItemCallback != null;
            _allowRemove = allowRemove;
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
                        if (_allowUpdate)
                            UpdateDomainItem(foundDomainItem, sourceItem);

                        // and remove from un-processed list
                        unProcessed.Remove(foundDomainItem);
                    }
                });

            // Remove any unprocessed items from the domain list
            if (_allowRemove && unProcessed.Count > 0)
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
