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

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Synchronizes the state of one collection based on the state of another collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The term "synchronization" here has nothing to do with threads, but refers to updating the elements of
    /// one collection based on the elements contained in another collection.  The two collections need not 
    /// have the same element type.
    /// </para>
    /// <para>
    /// There are two ways to use this class.  Either instantiate it directly, providing a set of delegates
    /// to customize the behaviour, or create a subclass and override methods to customize behaviour.
    /// </para>
    /// </remarks>
    /// <typeparam name="TDestItem"></typeparam>
    /// <typeparam name="TSourceItem"></typeparam>
    public class CollectionSynchronizeHelper<TDestItem, TSourceItem>
        where TDestItem : class
    {
        public delegate bool CompareItemsDelegate(TDestItem domainItem, TSourceItem sourceItem);
        public delegate TDestItem CreateDestItemDelegate(TSourceItem item);
        public delegate void UpdateDestItemDelegate(TDestItem domainItem, TSourceItem sourceItem);
        public delegate void RemoveDestItemDelegate(ICollection<TDestItem> domainList, TDestItem domainItem);

        private readonly CompareItemsDelegate _compareItemsCallback;
        private readonly CreateDestItemDelegate _createDestItemCallback;
        private readonly UpdateDestItemDelegate _updateDestItemCallback;
        private readonly RemoveDestItemDelegate _removeDestItemCallback;

        private readonly bool _allowUpdate = false;
        private readonly bool _allowRemove = false;

        /// <summary>
        /// Protected constructor for subclasses.
        /// </summary>
        protected CollectionSynchronizeHelper(bool allowUpdate, bool allowRemove)
        {
            _allowUpdate = allowUpdate;
            _allowRemove = allowRemove;
        }

        /// <summary>
        /// Public constructor allows direct use of this class without the need to create a subclass.
        /// </summary>
        public CollectionSynchronizeHelper(
            CompareItemsDelegate compareItemsCallback,
            CreateDestItemDelegate createDomainItemCallback,
            UpdateDestItemDelegate updateDomainItemCallback,
            RemoveDestItemDelegate removeDomainItemCallback)
        {
            _compareItemsCallback = compareItemsCallback;
            _createDestItemCallback = createDomainItemCallback;
            _updateDestItemCallback = updateDomainItemCallback;
            _removeDestItemCallback = removeDomainItemCallback;

            _allowUpdate = _updateDestItemCallback != null;
            _allowRemove = _removeDestItemCallback != null;
        }

        /// <summary>
        /// Synchronize the destList to match the sourceList.
        /// </summary>
        /// <param name="destList"></param>
        /// <param name="sourceList"></param>
        public void Synchronize(ICollection<TDestItem> destList, ICollection<TSourceItem> sourceList)
        {
            IList<TDestItem> unProcessed = new List<TDestItem>(destList);

            CollectionUtils.ForEach(sourceList,
                    delegate(TSourceItem sourceItem)
                    {
                        // Find a dest item that matches the source item
                        TDestItem foundDestItem = CollectionUtils.SelectFirst(destList,
                                                           delegate(TDestItem domainItem) { return CompareItems(domainItem, sourceItem); });

                        if (foundDestItem == null)
                        {
                            // Add a new dest item
                            destList.Add(CreateDestItem(sourceItem));
                        }
                        else
                        {
                            // Update the existing attachment
                            if (_allowUpdate)
                                UpdateDestItem(foundDestItem, sourceItem);

                            // and remove from un-processed list
                            unProcessed.Remove(foundDestItem);
                        }
                    });

            // Any items in the dest list that are not in the source list are considered "deleted"
            if (unProcessed.Count > 0)
            {
                if (_allowRemove)
                {
                    CollectionUtils.ForEach(unProcessed,
                                            delegate(TDestItem destItem)
                                            {
                                                RemoveDestItem(destList, destItem);
                                            });
                }
            }
        }

        protected virtual bool CompareItems(TDestItem destItem, TSourceItem sourceItem)
        {
            if (_compareItemsCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");

            return _compareItemsCallback(destItem, sourceItem);
        }

        protected virtual TDestItem CreateDestItem(TSourceItem sourceItem)
        {
            if (_createDestItemCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");

            return _createDestItemCallback(sourceItem);
        }

        protected virtual void UpdateDestItem(TDestItem destItem, TSourceItem sourceItem)
        {
            if (_updateDestItemCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");

            _updateDestItemCallback(destItem, sourceItem);
        }

        protected virtual void RemoveDestItem(ICollection<TDestItem> destList, TDestItem domainItem)
        {
            if (_removeDestItemCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");

            _removeDestItemCallback(destList, domainItem);
        }
    }
}