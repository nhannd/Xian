using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailRepository
    {
        bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail);
        Bitmap GetThumbnail(ThumbnailDescriptor descriptor, Size size);
        Bitmap GetDummyThumbnail(string message, Size size);
    }

    public class LegacyRepository : IThumbnailRepository
    {
        private readonly IThumbnailFactory _factory;

        public LegacyRepository()
        {
            _factory = new ThumbnailFactory();
        }

        #region IThumbnailRepository Members

        public bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail)
        {
            thumbnail = null;
            return false;
        }

        public Bitmap GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            return _factory.CreateThumbnail(descriptor.ReferenceImage, size);
        }

        public Bitmap GetDummyThumbnail(string message, Size size)
        {
            return _factory.CreateDummy(message, size);
        }

        #endregion
    }

    public class ThumbnailRepository : IThumbnailRepository
    {
        private readonly IThumbnailCache _cache = ThumbnailCache.Create("viewer-display-sets");
        private readonly IThumbnailFactory _thumbnailFactory;

        public ThumbnailRepository()
            : this(new ThumbnailFactory())
        {
        }

        public ThumbnailRepository(IThumbnailFactory thumbnailFactory)
        {
            _thumbnailFactory = thumbnailFactory;
        }

        public Bitmap GetDummyThumbnail(string message, Size size)
        {
            //TODO: bother caching these, seeing as we end up creating copies of them anyway?
            return _thumbnailFactory.CreateDummy(message, size);
        }

        public bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail)
        {
            string key = descriptor.Identifier;
            if (!String.IsNullOrEmpty(key))
            {
                var thumbnailData = _cache.Get(key);
                if (thumbnailData != null)
                {
                    thumbnail = thumbnailData.ToBitmap();
                    return true;
                }
            }

            thumbnail = null;
            return false;
        }

        public Bitmap GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            string key = descriptor.Identifier;
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Thumbnail descriptor must have a unique identifier.");

            var thumbnailData = _cache.Get(key);
            if (thumbnailData != null)
                return thumbnailData.ToBitmap();

            var image = _thumbnailFactory.CreateThumbnail(descriptor.ReferenceImage, size);
            _cache.Put(key, ThumbnailData.FromBitmap(image));
            return image;
        }
    }
}