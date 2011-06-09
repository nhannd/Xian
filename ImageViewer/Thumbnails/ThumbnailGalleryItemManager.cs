using System;
using System.Drawing;
using System.Threading;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailGalleryItemManager
    {
        void InitializeThumbnail(ThumbnailGalleryItem item, Size thumbnailSize);
        void ThumbnailDisposed(ThumbnailGalleryItem item);
        void Reset();
    }

    public class ThumbnailGalleryItemManager : IThumbnailGalleryItemManager
    {
        private class ThumbnailGalleryItemBinding
        {
            private readonly ThumbnailGalleryItemManager _manager;

            private readonly IDisplaySet _displaySet;
            private readonly ThumbnailDescriptor _descriptor;
            private ThumbnailGalleryItem _thumbnail;
            private readonly Size _thumbnailSize;

            public ThumbnailGalleryItemBinding(ThumbnailGalleryItemManager manager, ThumbnailGalleryItem thumbnail, Size thumbnailSize)
            {
                _manager = manager;
                _displaySet = (IDisplaySet)thumbnail.Item;
                _descriptor = ThumbnailDescriptor.Create(_displaySet, true);

                _thumbnail = thumbnail;
                _thumbnailSize = thumbnailSize;
                InitializeThumbnail();
            }

            private SynchronizationContext SynchronizationContext { get { return _manager._synchronizationContext; } }
            private IThumbnailLoader Loader { get { return _manager._loader; } }

            private void InitializeThumbnail()
            {
                _thumbnail.Name = GetDisplaySetName();
                if (_descriptor == null)
                {
                    _thumbnail.Image = Loader.GetDummyThumbnail(_manager.NoImagesMessage, _thumbnailSize);
                }
                else
                {
                    Bitmap image;
                    if (Loader.TryGetThumbnail(_descriptor, _thumbnailSize, out image))
                    {
                        _thumbnail.Image = image;
                    }
                    else
                    {
                        _thumbnail.Image = Loader.GetDummyThumbnail(_manager.LoadingMessage, _thumbnailSize);
                        Loader.LoadThumbnailAsync(new LoadThumbnailRequest(_descriptor, _thumbnailSize, OnThumbnailLoaded));
                    }
                }
            }

            private string GetDisplaySetName()
            {
                string name = _displaySet.Name;
                name = name.Replace("\r\n", " ");
                name = name.Replace("\r", " ");
                name = name.Replace("\n", " ");

                int number = _displaySet.PresentationImages.Count;
                if (number <= 1)
                    return String.Format(SR.FormatThumbnailName1Image, name);

                return String.Format(SR.FormatThumbnailName, number, name);
            }

            private void OnThumbnailLoaded(LoadThumbnailResult result)
            {
                if (!SynchronizationContext.Equals(SynchronizationContext.Current))
                {
                    SynchronizationContext.Post(ignore => OnThumbnailLoaded(result), null);
                }
                else
                {
                    //We have to clean up our own mess here.
                    _descriptor.ReferenceImage.Dispose();

                    if (result.Image != null)
                    {
                        if (_thumbnail.IsDisposed)
                            result.Image.Dispose();
                        else
                            _thumbnail.Image = result.Image;
                    }
                    else if (!_thumbnail.IsDisposed)
                    {
                        _thumbnail.Image = Loader.GetDummyThumbnail(_manager.LoadFailedMessage, _thumbnailSize);
                    }
                }
            }
        }

        private readonly IThumbnailLoader _loader;
        private readonly SynchronizationContext _synchronizationContext;

        public ThumbnailGalleryItemManager()
            : this(new ThumbnailLoader())
        {
        }

        public ThumbnailGalleryItemManager(IThumbnailLoader loader)
        {
            _synchronizationContext = SynchronizationContext.Current;
            if (_synchronizationContext == null)
                throw new InvalidOperationException("It is expected that the factory will be instantiated on and accessed from a UI thread.");

            _loader = loader;
        }

        private string LoadingMessage { get { return SR.MessageLoading; } }
        private string LoadFailedMessage { get { return SR.MessageLoadFailed; } }
        private string NoImagesMessage { get { return SR.MessageNoImages; } }

        #region IThumbnailGalleryItemManager<IDisplaySet> Members

        public void InitializeThumbnail(ThumbnailGalleryItem item, Size thumbnailSize)
        {
            new ThumbnailGalleryItemBinding(this, item, thumbnailSize);
        }

        public void ThumbnailDisposed(ThumbnailGalleryItem item)
        {
        }

        public void Reset()
        {
            _loader.Reset();
        }

        #endregion
    }
}