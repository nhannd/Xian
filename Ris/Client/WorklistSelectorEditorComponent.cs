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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistSelectorEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistSelectorEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public abstract class WorklistSelectorEditorComponent : ApplicationComponent
    {
        public abstract ITable AvailableItemsTable { get; }
        public abstract ITable SelectedItemsTable { get; }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }
    }

    /// <summary>
    /// WorklistSelectorEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistSelectorEditorComponentViewExtensionPoint))]
    public class WorklistSelectorEditorComponent<TSummary, TTable> : WorklistSelectorEditorComponent
        where TSummary : DataContractBase
        where TTable : Table<TSummary>, new()
    {
        private readonly TTable _available;
        private readonly TTable _selected;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistSelectorEditorComponent(IEnumerable<TSummary> allItems, IEnumerable<TSummary> selectedItems, Converter<TSummary, EntityRef> identityProvider)
        {
            _available = new TTable();
            _selected = new TTable();

            _selected.Items.AddRange(selectedItems);
            _available.Items.AddRange(Subtract(selectedItems, allItems, identityProvider));
        }

		/// <summary>
		/// Gets or sets the list of all possible items.
		/// </summary>
		public List<TSummary> AllItems
		{
			get
			{
				List<TSummary> list = new List<TSummary>(_available.Items);
				list.AddRange(_selected.Items);
				return list;
			}
			set
			{
				_available.Items.Clear();
				
				if(value != null)
					_available.Items.AddRange(value);

				// remove any selected items that are no longer valid choices,
				// and any available items that are already selected
				List<TSummary> selectedItems = new List<TSummary>(_selected.Items);
				foreach (TSummary item in selectedItems)
				{
					if (_available.Items.Contains(item))
						_available.Items.Remove(item);
					else
						_selected.Items.Remove(item);
				}
			}
		}

		/// <summary>
		/// Gets the list of selected items.
		/// </summary>
		public IList<TSummary> SelectedItems
        {
            get { return _selected.Items; }
        }

        #region Presentation Model

        public override ITable AvailableItemsTable
        {
            get { return _available; }
        }

        public override ITable SelectedItemsTable
        {
            get { return _selected; }
        }

        #endregion

        private static List<T> Subtract<T>(IEnumerable<T> some, IEnumerable<T> all, Converter<T, EntityRef> identityProvider)
        {
            return CollectionUtils.Reject(all,
                        delegate(T x)
                        {
                            return CollectionUtils.Contains(some,
                                delegate(T y) { return identityProvider(x).Equals(identityProvider(y), true); });
                        });
        }
    }
}
