#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Generic abstract base class for collections of <see cref="DesktopObject"/> subclasses.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="DesktopObject"/> subclass.</typeparam>
    public abstract class DesktopObjectCollection<T> : IEnumerable<T>, IDisposable
        where T : DesktopObject
    {

        private List<T> _list;
        private Dictionary<string, T> _nameMap;

        private event EventHandler<ItemEventArgs<T>> _itemOpening;
        private event EventHandler<ItemEventArgs<T>> _itemOpened;
        private event EventHandler<ClosingItemEventArgs<T>> _itemClosing;
        private event EventHandler<ClosedItemEventArgs<T>> _itemClosed;
        private event EventHandler<ItemEventArgs<T>> _itemActivationChanged;
        private event EventHandler<ItemEventArgs<T>> _itemVisibilityChanged;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected DesktopObjectCollection()
        {
            _list = new List<T>();
            _nameMap = new Dictionary<string, T>();
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~DesktopObjectCollection()
        {
            Dispose(false);
        }

        #region Public properties

        /// <summary>
        /// Gets the object in the collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the desktop object.</param>
        public T this[string name]
        {
            get
            {
                return _nameMap[name];
            }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks if the specified item exists in this collection.
        /// </summary>
        /// <param name="obj">The desktop object to look for.</param>
        public bool Contains(T obj)
        {
            return _list.Contains(obj);
        }

        /// <summary>
        /// Checks if this collection contains an item with the specified name.
        /// </summary>
        /// <param name="name">The name of the object to look for.</param>
        public bool Contains(string name)
        {
            return _nameMap.ContainsKey(name);
        }

        #endregion

        #region Public events

        /// <summary>
        /// Occurs when a new item is about to open, after it has been added to the collection.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemOpening
        {
            add { _itemOpening += value; }
            remove { _itemOpening -= value; }
        }

        /// <summary>
        /// Occurs after a new item has opened.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemOpened
        {
            add { _itemOpened += value; }
            remove { _itemOpened -= value; }
        }

        /// <summary>
        /// Occurs before an item is about to close.
        /// </summary>
        public event EventHandler<ClosingItemEventArgs<T>> ItemClosing
        {
            add { _itemClosing += value; }
            remove { _itemClosing -= value; }
        }
        
        /// <summary>
        /// Occurs after an item has closed and been removed from the collection. 
        /// </summary>
        public event EventHandler<ClosedItemEventArgs<T>> ItemClosed
        {
            add { _itemClosed += value; }
            remove { _itemClosed -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="DesktopObject.Visible"/> property of an item in the collection changes.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemVisibilityChanged
        {
            add { _itemVisibilityChanged += value; }
            remove { _itemVisibilityChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="DesktopObject.Active"/> property of an item in the collection changes.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemActivationChanged
        {
            add { _itemActivationChanged += value; }
            remove { _itemActivationChanged -= value; }
        }

        #endregion

        #region IEnumerable<T> Members

		/// <summary>
		/// Gets an <see cref="IEnumerator{T}"/> for the collection.
		/// </summary>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

		/// <summary>
		/// Gets an <see cref="System.Collections.IEnumerator"/> for the collection.
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
        void IDisposable.Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion

        #region Protected overridables

        /// <summary>
        /// Raises the <see cref="ItemOpening"/> event.
        /// </summary>
        protected virtual void OnItemOpening(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemOpening, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemOpened"/> event.
        /// </summary>
        protected virtual void OnItemOpened(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemOpened, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemClosing"/> event.
        /// </summary>
        protected virtual void OnItemClosing(ClosingItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemClosing, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemClosed"/> event.
        /// </summary>
        protected virtual void OnItemClosed(ClosedItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemClosed, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemVisibilityChanged"/> event.
        /// </summary>
        protected virtual void OnItemVisibilityChanged(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemVisibilityChanged, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemActivationChanged"/> event.
        /// </summary>
        protected virtual void OnItemActivationChangedInternal(ItemEventArgs<T> args)
        {
            // default behaviour, just raise the public event
            args.Item.RaiseActiveChanged();
        }

        /// <summary>
        /// Raises the <see cref="ItemActivationChanged"/> event.
        /// </summary>
        protected virtual void OnItemActivationChanged(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemActivationChanged, this, args);
        }

        /// <summary>
        /// Disposes of this collection, first disposing of each object in the collection.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (T obj in _list)
                {
                    try
                    {
                        (obj as IDisposable).Dispose();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e);
                    }
                }

                _nameMap.Clear();
                _list.Clear();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Opens the specified object.
        /// </summary>
        protected void Open(T obj)
        {
            obj.Opening += delegate(object sender, EventArgs e)
            {
                Add((T)sender);
                OnItemOpening(new ItemEventArgs<T>((T)sender));
            };
            obj.Opened += delegate(object sender, EventArgs e)
            {
                OnItemOpened(new ItemEventArgs<T>((T)sender));
            };
            obj.Closing += delegate(object sender, ClosingEventArgs e)
            {
                ClosingItemEventArgs<T> args = new ClosingItemEventArgs<T>((T)sender, e.Reason, e.Interaction, e.Cancel);
                OnItemClosing(args);
                e.Cancel = args.Cancel;
            };
            obj.Closed += delegate(object sender, ClosedEventArgs e)
            {
                Remove((T)sender);
                OnItemClosed(new ClosedItemEventArgs<T>((T)sender, e.Reason));
            };
            obj.VisibleChanged += delegate(object sender, EventArgs e)
            {
                OnItemVisibilityChanged(new ItemEventArgs<T>((T)sender));
            };
            obj.InternalActiveChanged += delegate(object sender, EventArgs e)
            {
                OnItemActivationChangedInternal(new ItemEventArgs<T>((T)sender));
            };
            obj.ActiveChanged += delegate(object sender, EventArgs e)
            {
                OnItemActivationChanged(new ItemEventArgs<T>((T)sender));
            };

            obj.Open();
        }

        /// <summary>
        /// Adds the specified object to the collection.
        /// </summary>
        private void Add(T obj)
        {
            if (_list.Contains(obj))
                throw new InvalidOperationException(SR.ExceptionObjectAlreadyInCollection);
            if (obj.Name != null && _nameMap.ContainsKey(obj.Name))
                throw new InvalidOperationException(string.Format(SR.ExceptionObjectWithNameAlreadyExists, obj.Name));

            _list.Add(obj);
            if (obj.Name != null)
            {
                _nameMap.Add(obj.Name, obj);
            }
        }

        /// <summary>
        /// Removes the specified object from the collection.
        /// </summary>
        private void Remove(T obj)
        {
            if (_nameMap.ContainsValue(obj))
                _nameMap.Remove(obj.Name);
            _list.Remove(obj);
        }

        #endregion
    }
}
