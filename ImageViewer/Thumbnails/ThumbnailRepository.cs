using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailRepository
    {
        Bitmap GetDummyThumbnail(string message, Size size);

        bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail);
        Bitmap GetThumbnail(ThumbnailDescriptor descriptor, Size size);
    }

    public abstract class ThumbnailRepository : IThumbnailRepository
    {
        #region IThumbnailRepository Members

        public abstract Bitmap GetDummyThumbnail(string message, Size size);

        public abstract bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail);
        public abstract Bitmap GetThumbnail(ThumbnailDescriptor descriptor, Size size);

        #endregion

        public static IThumbnailRepository Create()
        {
            //TODO:!change back to use caching
            //if (ThumbnailCache.IsSupported)
            //    return new CachingThumbnailRepository(ThumbnailCache.Create("viewer-display-sets"));

            return new CachelessThumbnailRepository();
        }
    }

    public class CachelessThumbnailRepository : ThumbnailRepository
    {
        private readonly IThumbnailFactory _factory;

        public CachelessThumbnailRepository()
            : this(new ThumbnailFactory())
        {
        }
        public CachelessThumbnailRepository(IThumbnailFactory factory)
        {
            Platform.CheckForNullReference(factory, "factory");
            _factory = factory;
        }

        public override bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail)
        {
            thumbnail = null;
            return false;
        }

        public override Bitmap GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            return _factory.CreateThumbnail(descriptor.ReferenceImage, size);
        }

        public override Bitmap GetDummyThumbnail(string message, Size size)
        {
            return _factory.CreateDummy(message, size);
        }
    }

    public class CachingThumbnailRepository : ThumbnailRepository
    {
        private readonly IThumbnailCache _cache;
        private readonly IThumbnailFactory _factory;

        internal CachingThumbnailRepository(IThumbnailCache cache)
            : this(new ThumbnailFactory(), cache)
        {
        }

        public CachingThumbnailRepository(IThumbnailFactory factory, IThumbnailCache cache)
        {
            Platform.CheckForNullReference(factory, "factory");
            Platform.CheckForNullReference(cache, "cache");

            _factory = factory;
            _cache = cache;
        }

        public override Bitmap GetDummyThumbnail(string message, Size size)
        {
            //TODO: bother caching these, seeing as we end up creating copies of them anyway?
            return _factory.CreateDummy(message, size);
        }

        public override bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out Bitmap thumbnail)
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

        public override Bitmap GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            string key = descriptor.Identifier;
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Thumbnail descriptor must have a unique identifier.");

            var thumbnailData = _cache.Get(key);
            if (thumbnailData != null)
                return thumbnailData.ToBitmap();

            var image = _factory.CreateThumbnail(descriptor.ReferenceImage, size);
            _cache.Put(key, ThumbnailData.FromBitmap(image));
            return image;
        }
    }
}