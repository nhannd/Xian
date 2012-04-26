using System;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    internal static class Cache<T> where T : class 
    {
        private class CacheItem
        {
            private readonly object _lock = new object();
            private DateTime? _cachedTime;
            private T _value;

            public T Value
            {
                get
                {
                    lock (_lock)
                    {
                        if (!_cachedTime.HasValue)
                            return null;

                        var elapsed = DateTime.Now - _cachedTime.Value;
                        if (elapsed > TimeSpan.FromSeconds(30))
                            _value = null;

                        return _value;
                    }
                }
                set
                {
                    lock (_lock)
                    {
                        if (value == null)
                        {
                            _cachedTime = null;
                            _value = null;
                            return;
                        }

                        _value = value;
                        _cachedTime = DateTime.Now;
                    }
                }
            }
        }

        private static readonly CacheItem _cacheItem = new CacheItem();

        public static T CachedValue
        {
            get { return _cacheItem.Value; }
            set { _cacheItem.Value = value; }
        }
    }
}
