using System;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class ThumbnailGallery<T> : IDisposable where T : class
    {
        private readonly IThumbnailGalleryItemManager _thumbnailManager;
        private readonly Size _thumbnailSize;
        private IObservableList<T> _sourceItems;
        private int _lastChangedIndex = -1;

        public ThumbnailGallery()
            : this(new ThumbnailGalleryItemManager(), ThumbnailSizes.Medium)
        {
        }

        public ThumbnailGallery(IThumbnailGalleryItemManager thumbnailManager, Size thumbnailSize)
        {
            Platform.CheckForNullReference(thumbnailManager, "thumbnailManager");

            _thumbnailManager = thumbnailManager;
            _thumbnailSize = thumbnailSize;
            Thumbnails = new BindingList<IGalleryItem>();
        }

        public BindingList<IGalleryItem> Thumbnails { get; private set; }

        public IObservableList<T> SourceItems
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
                _thumbnailManager.Reset();

                foreach (ThumbnailGalleryItem thumbnail in Thumbnails)
                    DisposeThumbnail(thumbnail);

                Thumbnails.Clear();

                if (_sourceItems == null)
                    return;

                _sourceItems.ItemAdded += OnSourceItemAdded;
                _sourceItems.ItemChanging += OnSourceItemChanging;
                _sourceItems.ItemChanged += OnSourceItemChanged;
                _sourceItems.ItemRemoved += OnSourceItemRemoved;

                foreach (var displaySet in _sourceItems)
                    Thumbnails.Add(CreateNew(displaySet));
            }
        }

        protected virtual void OnThumbnailItemUpdated(ThumbnailGalleryItem thumbnail, int index)
        {
            Thumbnails.ResetItem(index);
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

        private void OnSourceItemAdded(object sender, ListEventArgs<T> e)
        {
            Thumbnails.Add(CreateNew(e.Item));
        }

        private void OnSourceItemChanging(object sender, ListEventArgs<T> e)
        {
            _lastChangedIndex = IndexOf(e.Item);
        }

        private void OnSourceItemChanged(object sender, ListEventArgs<T> e)
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

        private void OnSourceItemRemoved(object sender, ListEventArgs<T> e)
        {
            var index = IndexOf(e.Item);
            if (index < 0)
                return;

            var thumbnail = (ThumbnailGalleryItem)Thumbnails[index];
            Thumbnails.RemoveAt(index);
            DisposeThumbnail(thumbnail);
        }

        private int IndexOf(T item)
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

        private ThumbnailGalleryItem CreateNew(T item)
        {
            var thumbnail = new ThumbnailGalleryItem(item);
            _thumbnailManager.InitializeThumbnail(thumbnail, _thumbnailSize);
            thumbnail.PropertyChanged += OnThumbnailPropertyChanged;
            return thumbnail;
        }

        private void DisposeThumbnail(ThumbnailGalleryItem thumbnail)
        {
            thumbnail.PropertyChanged -= OnThumbnailPropertyChanged;
            _thumbnailManager.ThumbnailDisposed(thumbnail);
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
