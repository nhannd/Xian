using System;
using System.Drawing;
using System.Threading;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailGalleryItemManager
    {
        void Initialize(ThumbnailGalleryItem item, Size thumbnailSize);
        void Destroy(ThumbnailGalleryItem item);
    }

    public class ThumbnailGalleryItemManager : IThumbnailGalleryItemManager
    {
        private class ThumbnailGalleryItemState
        {
            private readonly ThumbnailGalleryItemManager _manager;

            private readonly IDisplaySet _displaySet;
            private readonly ThumbnailGalleryItem _thumbnail;
            private readonly Size _thumbnailSize;

            private LoadThumbnailRequest _pendingRequest;

            public ThumbnailGalleryItemState(ThumbnailGalleryItemManager manager, ThumbnailGalleryItem thumbnail, Size thumbnailSize)
            {
                _manager = manager;
                _displaySet = (IDisplaySet)thumbnail.Item;
                _thumbnail = thumbnail;
                _thumbnailSize = thumbnailSize;
                
                Initialize();
            }

            private SynchronizationContext SynchronizationContext { get { return _manager._synchronizationContext; } }
            private IThumbnailLoader Loader { get { return _manager._loader; } }

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

            private void Initialize()
            {
                _thumbnail.State = this;
                _thumbnail.Name = GetDisplaySetName();

                var descriptor = ThumbnailDescriptor.Create(_displaySet, true);
                if (descriptor == null)
                {
                    //Is this strictly true?
                    _thumbnail.ImageData = Loader.GetDummyThumbnail(_manager.NoImagesMessage, _thumbnailSize);
                }
                else
                {
                    IThumbnailData imageData;
                    if (Loader.TryGetThumbnail(descriptor, _thumbnailSize, out imageData))
                    {
                        _thumbnail.ImageData = imageData;
                    }
                    else
                    {
                        _thumbnail.ImageData = Loader.GetDummyThumbnail(_manager.LoadingMessage, _thumbnailSize);

                        _thumbnail.PropertyChanged += OnThumbnailPropertyChanged;
                        _pendingRequest = new LoadThumbnailRequest(descriptor, _thumbnailSize, OnThumbnailLoaded);
                        Loader.LoadThumbnailAsync(new LoadThumbnailRequest(descriptor, _thumbnailSize, OnThumbnailLoaded));
                    }
                }
            }

            private void OnThumbnailPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "IsVisible" && _pendingRequest != null)
                {
                    var thumbnail = (ThumbnailGalleryItem)sender;
                    if(!thumbnail.IsVisible)
                        Loader.Cancel(_pendingRequest);
                    else
                        Loader.LoadThumbnailAsync(_pendingRequest);
                }
            }

            private void OnThumbnailLoaded(LoadThumbnailResult result)
            {
                if (!SynchronizationContext.Equals(SynchronizationContext.Current))
                {
                    SynchronizationContext.Post(ignore => OnThumbnailLoaded(result), null);
                }
                else
                {
                    _pendingRequest.Descriptor.ReferenceImage.Dispose();
                    _pendingRequest = null;

                    if (_thumbnail.State == null)
                    {
                        if (_thumbnail.ImageData != null)
                            _thumbnail.ImageData.Dispose();
                    }

                    else
                    {
                        Dispose();
                        _thumbnail.ImageData = result.ThumbnailData ?? Loader.GetDummyThumbnail(_manager.LoadFailedMessage, _thumbnailSize);
                    }
                }
            }

            public void Dispose()
            {
                if (_thumbnail.State == null)
                    return;

                _thumbnail.State = null;
                _thumbnail.PropertyChanged -= OnThumbnailPropertyChanged;
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

        public void Initialize(ThumbnailGalleryItem item, Size thumbnailSize)
        {
            new ThumbnailGalleryItemState(this, item, thumbnailSize);
        }

        public void Destroy(ThumbnailGalleryItem item)
        {
            var state = item.State as ThumbnailGalleryItemState;
            if (state != null)
                state.Dispose();
        }

        #endregion
    }
}