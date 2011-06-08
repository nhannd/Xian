using System;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System.Threading;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class ThumbnailGallery : IDisposable
    {
        private IImageSet _imageSet;
        private int _lastChangedIndex = -1;

        private readonly Size _thumbnailSize;

        private readonly IThumbnailLoader _loader;
        private readonly SynchronizationContext _synchronizationContext;

        public ThumbnailGallery()
            : this(new ThumbnailLoader(), ThumbnailSizes.Medium)
        {
        }

        public ThumbnailGallery(Size thumbnailSize)
            : this(new ThumbnailLoader(), thumbnailSize)
        {
        }

        public ThumbnailGallery(IThumbnailLoader loader, Size thumbnailSize)
        {
            _synchronizationContext = SynchronizationContext.Current;
            if (_synchronizationContext == null)
                throw new InvalidOperationException("It is expected that the gallery will be instantiated on and accessed from a UI thread.");

            _loader = loader;

            _thumbnailSize = thumbnailSize;
            Thumbnails = new BindingList<IGalleryItem>();
        }

        public Size ThumbnailSize { get { return _thumbnailSize; } }

        public BindingList<IGalleryItem> Thumbnails;

        public IImageSet ImageSet
        {
            get { return _imageSet; }
            set
            {
                if (Equals(_imageSet, value))
                    return;

                if (_imageSet != null)
                {
                    _imageSet.DisplaySets.ItemAdded -= OnDisplaySetAdded;
                    _imageSet.DisplaySets.ItemChanging -= OnDisplaySetChanging;
                    _imageSet.DisplaySets.ItemChanged -= OnDisplaySetChanged;
                    _imageSet.DisplaySets.ItemRemoved -= OnDisplaySetRemoved;
                }

                _imageSet = value;
                _loader.Reset();

                foreach (ThumbnailGalleryItem thumbnail in Thumbnails)
                    DisposeThumbnail(thumbnail);

                Thumbnails.Clear();

                if (_imageSet == null)
                    return;

                _imageSet.DisplaySets.ItemAdded += OnDisplaySetAdded;
                _imageSet.DisplaySets.ItemChanging += OnDisplaySetChanging;
                _imageSet.DisplaySets.ItemChanged += OnDisplaySetChanged;
                _imageSet.DisplaySets.ItemRemoved += OnDisplaySetRemoved;

                foreach (var displaySet in _imageSet.DisplaySets)
                    Thumbnails.Add(CreateNew(displaySet));
            }
        }

        private static void DisposeThumbnail(ThumbnailGalleryItem thumbnail)
        {
            if (thumbnail.Descriptor.ReferenceImage != null)
                thumbnail.Descriptor.ReferenceImage.Dispose();

            thumbnail.Dispose();
        }

        private string LoadingMessage { get { return SR.MessageLoading; } }
        private string LoadFailedMessage { get { return SR.MessageLoadFailed; } }
        private string NoImagesMessage { get { return SR.MessageNoImages; } }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            ImageSet = null;
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

        private void OnDisplaySetAdded(object sender, ListEventArgs<IDisplaySet> e)
        {
            var thumbnail = CreateNew(e.Item);
            Thumbnails.Add(thumbnail);
        }

        private void OnDisplaySetChanging(object sender, ListEventArgs<IDisplaySet> e)
        {
            _lastChangedIndex = IndexOf(e.Item);
        }

        private void OnDisplaySetChanged(object sender, ListEventArgs<IDisplaySet> e)
        {
            if (_lastChangedIndex >= 0)
            {
                var oldThumbnail = (ThumbnailGalleryItem)Thumbnails[_lastChangedIndex];
                var newThumbnail = CreateNew(e.Item);
                Thumbnails[_lastChangedIndex] = newThumbnail;
                DisposeThumbnail(oldThumbnail);
                Thumbnails.ResetItem(_lastChangedIndex);
            }
            else
            {
                var thumbnail = CreateNew(e.Item);
                Thumbnails.Add(thumbnail);
            }
        }

        private void OnDisplaySetRemoved(object sender, ListEventArgs<IDisplaySet> e)
        {
            var index = IndexOf(e.Item);
            if (index < 0)
                return;

            var thumbnail = (IDisposable)Thumbnails[index];
            Thumbnails.RemoveAt(index);
            thumbnail.Dispose();
        }

        private int IndexOf(IDisplaySet displaySet)
        {
            int i = 0;
            foreach(IThumbnailGalleryItem thumbnail in Thumbnails)
            {
                if (thumbnail.DisplaySet == displaySet)
                    return i;
                ++i;
            }

            return -1;
        }

        private IThumbnailGalleryItem CreateNew(IDisplaySet displaySet)
        {
            var referenceImage = ThumbnailDescriptor.GetMiddlePresentationImage(displaySet);
            if (referenceImage != null)
                referenceImage = referenceImage.CreateFreshCopy();

            var descriptor = new ThumbnailDescriptor(displaySet, referenceImage);
            var item = new ThumbnailGalleryItem(descriptor);
            if (referenceImage == null)
            {
                item.Image = _loader.GetDummyThumbnail(NoImagesMessage, _thumbnailSize);
            }
            else
            {
                Bitmap image;
                if (_loader.TryLoadThumbnail(descriptor, _thumbnailSize, out image))
                {
                    item.Image = image;
                }
                else
                {
                    item.Image = _loader.GetDummyThumbnail(LoadingMessage, _thumbnailSize);
                    _loader.LoadThumbnail(new LoadThumbnailRequest(descriptor, _thumbnailSize, OnThumbnailLoaded));
                }
            }

            return item;
        }

        private void OnThumbnailLoaded(LoadThumbnailResult result)
        {
            if (!_synchronizationContext.Equals(SynchronizationContext.Current))
            {
                _synchronizationContext.Post(ignore => OnThumbnailLoaded(result), null);
            }
            else
            {
                //We created it as a fresh copy, so now we have to dispose it ... unconditionally.
                int index = IndexOf(result.Descriptor.DisplaySet);
                if (index < 0)
                {
                    if (result.Image != null) //Make sure it gets disposed.
                        result.Image.Dispose();
                }
                else
                {
                    var thumbnail = (ThumbnailGalleryItem)Thumbnails[index];
                    thumbnail.Image = result.Error != null 
                        ? _loader.GetDummyThumbnail(LoadFailedMessage, _thumbnailSize) 
                        : result.Image;
                    Thumbnails.ResetItem(index);
                }
            }
        }
    }
}
