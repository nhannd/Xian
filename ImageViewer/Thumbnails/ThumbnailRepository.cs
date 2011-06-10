using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailRepository
    {
        IThumbnailData GetDummyThumbnail(string message, Size size);

        bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IThumbnailData thumbnail);
        IThumbnailData GetThumbnail(ThumbnailDescriptor descriptor, Size size);
    }

    public abstract class ThumbnailRepository : IThumbnailRepository
    {
        #region IThumbnailRepository Members

        public abstract IThumbnailData GetDummyThumbnail(string message, Size size);

        public abstract bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IThumbnailData thumbnail);
        public abstract IThumbnailData GetThumbnail(ThumbnailDescriptor descriptor, Size size);

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
            : this(new BitmapThumbnailFactory())
        {
        }
        public CachelessThumbnailRepository(IThumbnailFactory factory)
        {
            Platform.CheckForNullReference(factory, "factory");
            _factory = factory;
        }

        public override bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IThumbnailData thumbnail)
        {
            thumbnail = null;
            return false;
        }

        public override IThumbnailData GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            return _factory.CreateThumbnail(descriptor.ReferenceImage, size);
        }

        public override IThumbnailData GetDummyThumbnail(string message, Size size)
        {
            return _factory.CreateDummy(message, size);
        }
    }

    public class CachingThumbnailRepository : ThumbnailRepository
    {
        private readonly ICache<IThumbnailData> _cache;
        private readonly IThumbnailFactory _factory;

        internal CachingThumbnailRepository(ICache<IThumbnailData> cache)
            : this(new BitmapThumbnailFactory(), cache)
        {
        }

        public CachingThumbnailRepository(IThumbnailFactory factory, ICache<IThumbnailData> cache)
        {
            Platform.CheckForNullReference(factory, "factory");
            Platform.CheckForNullReference(cache, "cache");

            _factory = factory;
            _cache = cache;
        }

        public override IThumbnailData GetDummyThumbnail(string message, Size size)
        {
            //TODO: bother caching these, seeing as we end up creating copies of them anyway?
            return _factory.CreateDummy(message, size);
        }

        public override bool TryGetThumbnail(ThumbnailDescriptor descriptor, Size size, out IThumbnailData thumbnail)
        {
            string key = descriptor.Identifier;
            if (!String.IsNullOrEmpty(key))
            {
                thumbnail = _cache.Get(key);
                if (thumbnail != null)
                    return true;
            }

            thumbnail = null;
            return false;
        }

        public override IThumbnailData GetThumbnail(ThumbnailDescriptor descriptor, Size size)
        {
            string key = descriptor.Identifier;
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Thumbnail descriptor must have a unique identifier.");

            var thumbnail = _cache.Get(key);
            if (thumbnail != null)
                return thumbnail;

            thumbnail = _factory.CreateThumbnail(descriptor.ReferenceImage, size);
            _cache.Put(key, thumbnail);
            return thumbnail;
        }
    }
}