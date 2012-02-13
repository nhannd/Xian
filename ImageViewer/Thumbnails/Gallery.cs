#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IGallery
    {
        IList<IGalleryItem> GalleryItems { get; }
    }

    public interface IGallery<TSourceItem> : IGallery
    {
        IGalleryItemFactory<TSourceItem> GalleryItemFactory { get; }
        IObservableList<TSourceItem> SourceItems { get; set; }
    }

    public enum NameAndDescriptionFormat
    {
        NameAndDescription,
        NoDescription,
        VerboseNameNoDescription,
        VerboseNameAndDescription
    }

    public struct GalleryItemCreationArgs<TSourceItem>
    {
        public TSourceItem SourceItem;
        public NameAndDescriptionFormat NameAndDescriptionFormat;
    }

    public interface IGalleryItemFactory<TSourceItem>
    {
        IGalleryItem Create(GalleryItemCreationArgs<TSourceItem> args);
    }

    public class Gallery<TSourceItem> : IGallery<TSourceItem> where TSourceItem : class, IDisposable
    {
        private IObservableList<TSourceItem> _sourceItems;
        private int _lastChangedIndex = -1;

        public Gallery()
            : this(new BindingList<IGalleryItem>())
        {
        }

        public Gallery(IList<IGalleryItem> galleryItems)
        {
            GalleryItems = galleryItems;
        }

        public Gallery(IList<IGalleryItem> galleryItems, IGalleryItemFactory<TSourceItem> galleryItemFactory)
            : this(galleryItems)
        {
            GalleryItemFactory = galleryItemFactory;
        }

        public NameAndDescriptionFormat NameAndDescriptionFormat { get; set; }

        #region IGallery<TSourceItem> Members

        public IObservableList<TSourceItem> SourceItems
        {
            get { return _sourceItems; }
            set
            {
                if (Equals(_sourceItems, value))
                    return;

                if (_sourceItems != null)
                {
                    _sourceItems.ItemAdded -= OnSourceItemAdded;
                    _sourceItems.ItemChanging -= OnSourceItemChanging;
                    _sourceItems.ItemChanged -= OnSourceItemChanged;
                    _sourceItems.ItemRemoved -= OnSourceItemRemoved;
                }

                _sourceItems = value;

                Clear();
                if (_sourceItems == null)
                    return;

                _sourceItems.ItemAdded += OnSourceItemAdded;
                _sourceItems.ItemChanging += OnSourceItemChanging;
                _sourceItems.ItemChanged += OnSourceItemChanged;
                _sourceItems.ItemRemoved += OnSourceItemRemoved;

                foreach (var sourceItem in _sourceItems)
                    GalleryItems.Add(CreateNew(sourceItem));
            }
        }

        protected IGalleryItemFactory<TSourceItem> GalleryItemFactory { get; set; }

        IGalleryItemFactory<TSourceItem> IGallery<TSourceItem>.GalleryItemFactory { get { return GalleryItemFactory; } }

        #endregion

        #region IGallery Members

        public IList<IGalleryItem> GalleryItems { get; private set; }

        #endregion

        private void OnSourceItemAdded(object sender, ListEventArgs<TSourceItem> e)
        {
            GalleryItems.Add(CreateNew(e.Item));
        }

        private void OnSourceItemChanging(object sender, ListEventArgs<TSourceItem> e)
        {
            _lastChangedIndex = IndexOf(e.Item);
        }

        private void OnSourceItemChanged(object sender, ListEventArgs<TSourceItem> e)
        {
            if (_lastChangedIndex >= 0)
            {
                var oldItem = GalleryItems[_lastChangedIndex];
                var newItem = CreateNew(e.Item);
                GalleryItems[_lastChangedIndex] = newItem;
                OnItemRemoved(oldItem);
                OnItemChanged(newItem);
            }
            else
            {
                //This is really an error condition, but it'll never happen anyway.
                GalleryItems.Add(CreateNew(e.Item));
            }
        }

        private void OnSourceItemRemoved(object sender, ListEventArgs<TSourceItem> e)
        {
            var index = IndexOf(e.Item);
            if (index < 0)
                return;

            var item = GalleryItems[index];
            GalleryItems.RemoveAt(index);
            OnItemRemoved(item);
        }

        private int IndexOf(TSourceItem item)
        {
            int i = 0;
            foreach(var galleryItem in GalleryItems)
            {
                if (galleryItem.Item == item)
                    return i;
                ++i;
            }

            return -1;
        }

        protected virtual IGalleryItem CreateNew(TSourceItem item)
        {
            return GalleryItemFactory.Create(new GalleryItemCreationArgs<TSourceItem> { SourceItem = item, NameAndDescriptionFormat = NameAndDescriptionFormat });
        }

        protected virtual void OnItemChanged(IGalleryItem item)
        {
        }

        protected virtual void OnItemRemoved(IGalleryItem item)
        {
            item.Dispose();
        }

        private void Clear()
        {
            foreach (var item in GalleryItems)
                OnItemRemoved(item);

            GalleryItems.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Clear();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
            }
        }

        #endregion
    }
}
