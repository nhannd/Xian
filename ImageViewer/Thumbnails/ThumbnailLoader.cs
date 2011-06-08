using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class LoadThumbnailRequest
    {
        public LoadThumbnailRequest(ThumbnailDescriptor descriptor, Size size, Action<LoadThumbnailResult> resultCallback)
        {
            Descriptor = descriptor;
            Size = size;
            ResultCallback = resultCallback;
        }

        public readonly ThumbnailDescriptor Descriptor;
        public readonly Size Size;
        public readonly Action<LoadThumbnailResult> ResultCallback;
    }

    public class LoadThumbnailResult
    {
        public LoadThumbnailResult(ThumbnailDescriptor descriptor, Image image)
        {
            Descriptor = descriptor;
            Image = image;
        }

        public LoadThumbnailResult(ThumbnailDescriptor descriptor, Exception error)
        {
            Descriptor = descriptor;
            Error = error;
        }

        public readonly ThumbnailDescriptor Descriptor;
        public readonly Image Image;
        public readonly Exception Error;
    }

    public interface IThumbnailLoader
    {
        Bitmap GetDummyThumbnail(string message, Size size);

        bool TryLoadThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail);
        void LoadThumbnail(LoadThumbnailRequest request);
        void Reset();
    }

    public class ThumbnailLoader : IThumbnailLoader
    {
        private readonly IThumbnailRepository _repository;
        private readonly object _syncLock = new object();
        private readonly Queue<LoadThumbnailRequest> _pendingRequests = new Queue<LoadThumbnailRequest>();
        private bool _isLoading;

        public ThumbnailLoader()
            : this(new LegacyRepository())
        {
        }

        public ThumbnailLoader(IThumbnailRepository repository)
        {
            _repository = repository;
        }

        #region IThumbnailLoader Members

        public Bitmap GetDummyThumbnail(string message, Size size)
        {
            return _repository.GetDummyThumbnail(message, size);
        }

        public bool TryLoadThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap bitmap)
        {
            return _repository.TryGetThumbnail(descriptor, size, out bitmap);
        }

        public void LoadThumbnail(LoadThumbnailRequest request)
        {
            lock (_syncLock)
            {
                _pendingRequests.Enqueue(request);
                if (!_isLoading)
                {
                    _isLoading = true;
                    ThreadPool.QueueUserWorkItem(Load, null);
                }
            }
        }

        public void Reset()
        {
            lock(_syncLock)
            {
                _pendingRequests.Clear();
                if (_isLoading)
                    Monitor.Wait(_syncLock);
            }
        }

        #endregion

        public void Load(object state)
        {
            while (true)
            {
                LoadThumbnailRequest request;
                lock (_syncLock)
                {
                    if (_pendingRequests.Count == 0)
                    {
                        _isLoading = false;
                        Monitor.Pulse(_syncLock);
                        break;
                    }

                    _isLoading = true;
                    request = _pendingRequests.Dequeue();
                }

                LoadThumbnailResult result;

                try
                {
                    var image = _repository.GetThumbnail(request.Descriptor, request.Size);
                    result = new LoadThumbnailResult(request.Descriptor, image);
                }
                catch (Exception e)
                {
                    result = new LoadThumbnailResult(request.Descriptor, e);
                }

                request.ResultCallback(result);
            }
        }
    }
}