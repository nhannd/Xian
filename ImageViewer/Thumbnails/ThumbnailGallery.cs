using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailGallery : IDisposable
    {
        bool IsVisible { get; set; }
        IList<IGalleryItem> Thumbnails { get; }
    }

    public class BindingListThumbnailGallery<TSourceItem> : ThumbnailGallery<TSourceItem> where TSourceItem : class
    {
        public BindingListThumbnailGallery()
            : base(new BindingList<IGalleryItem>(), new ThumbnailGalleryItemManager(), ThumbnailSizes.Medium)
        {
        }

        public BindingListThumbnailGallery(IThumbnailGalleryItemManager thumbnailManager, Size thumbnailSize)
            : base(new BindingList<IGalleryItem>(), thumbnailManager, thumbnailSize)
        {
        }

        protected override void OnThumbnailItemUpdated(ThumbnailGalleryItem thumbnail, int index)
        {
            ((BindingList<IGalleryItem>)Thumbnails).ResetItem(index);
        }
    }

    public class ThumbnailGallery<TSourceItem> : IThumbnailGallery where TSourceItem : class
    {
        private readonly IThumbnailGalleryItemManager _thumbnailManager;
        private readonly Size _thumbnailSize;
        private IObservableList<TSourceItem> _sourceItems;
        private int _lastChangedIndex = -1;
        private bool _isVisible;

        public ThumbnailGallery(IList<IGalleryItem> thumbnails)
            : this(thumbnails, new ThumbnailGalleryItemManager(), ThumbnailSizes.Medium)
        {
        }

        public ThumbnailGallery(IList<IGalleryItem> thumbnails, IThumbnailGalleryItemManager thumbnailManager, Size thumbnailSize)
        {
            Platform.CheckForNullReference(thumbnailManager, "thumbnailManager");
            Platform.CheckForNullReference(thumbnails, "thumbnails");

            _thumbnailManager = thumbnailManager;
            _thumbnailSize = thumbnailSize;
            Thumbnails = thumbnails;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible)
                    return;

                _isVisible = value;
                foreach (ThumbnailGalleryItem thumbnail in Thumbnails)
                    thumbnail.IsVisible = value;
            }
        }

        public IList<IGalleryItem> Thumbnails { get; private set; }

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

                foreach (ThumbnailGalleryItem thumbnail in Thumbnails)
                    DisposeThumbnail(thumbnail);

                Thumbnails.Clear();

                if (_sourceItems == null)
                    return;

                _sourceItems.ItemAdded += OnSourceItemAdded;
                _sourceItems.ItemChanging += OnSourceItemChanging;
                _sourceItems.ItemChanged += OnSourceItemChanged;
                _sourceItems.ItemRemoved += OnSourceItemRemoved;

                foreach (var sourceItem in _sourceItems)
                    Thumbnails.Add(CreateNew(sourceItem));
            }
        }

        protected virtual void OnThumbnailItemUpdated(ThumbnailGalleryItem thumbnail, int index)
        {
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                SourceItems = null;
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

        private void OnSourceItemAdded(object sender, ListEventArgs<TSourceItem> e)
        {
            Thumbnails.Add(CreateNew(e.Item));
        }

        private void OnSourceItemChanging(object sender, ListEventArgs<TSourceItem> e)
        {
            _lastChangedIndex = IndexOf(e.Item);
        }

        private void OnSourceItemChanged(object sender, ListEventArgs<TSourceItem> e)
        {
            if (_lastChangedIndex >= 0)
            {
                var oldThumbnail = (ThumbnailGalleryItem)Thumbnails[_lastChangedIndex];
                var newThumbnail = CreateNew(e.Item);
                Thumbnails[_lastChangedIndex] = newThumbnail;
                DisposeThumbnail(oldThumbnail);
                OnThumbnailItemUpdated(newThumbnail, _lastChangedIndex);
            }
            else
            {
                //This is really an error condition, but it'll never happen anyway.
                Thumbnails.Add(CreateNew(e.Item));
            }
        }

        private void OnSourceItemRemoved(object sender, ListEventArgs<TSourceItem> e)
        {
            var index = IndexOf(e.Item);
            if (index < 0)
                return;

            var thumbnail = (ThumbnailGalleryItem)Thumbnails[index];
            Thumbnails.RemoveAt(index);
            DisposeThumbnail(thumbnail);
        }

        private int IndexOf(TSourceItem item)
        {
            int i = 0;
            foreach(ThumbnailGalleryItem thumbnail in Thumbnails)
            {
                if (thumbnail.Item == item)
                    return i;
                ++i;
            }

            return -1;
        }

        private ThumbnailGalleryItem CreateNew(TSourceItem item)
        {
            var thumbnail = new ThumbnailGalleryItem(item);
            _thumbnailManager.Initialize(thumbnail, _thumbnailSize);
            thumbnail.PropertyChanged += OnThumbnailPropertyChanged;
            return thumbnail;
        }

        private void DisposeThumbnail(ThumbnailGalleryItem thumbnail)
        {
            thumbnail.PropertyChanged -= OnThumbnailPropertyChanged;
            _thumbnailManager.Destroy(thumbnail);
            thumbnail.Dispose();
        }

        private void OnThumbnailPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var thumbnail = (ThumbnailGalleryItem) sender;
            int index = Thumbnails.IndexOf(thumbnail);
            if (index < 0)
                return;

            OnThumbnailItemUpdated(thumbnail, index);
        }
    }
}
