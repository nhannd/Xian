#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailData<T> : IThumbnailData
    {
        new T Image { get; }    
    }

    public interface IThumbnailData : IDisposable
    {
        object Image { get; }
        IThumbnailData Clone();
    }

    internal class ThumbnailDataProxy<T> : IThumbnailData<T> where T : class
    {
        private ReferenceCountedObjectWrapper<IThumbnailData<T>> _real;

        public ThumbnailDataProxy(IThumbnailData<T> real)
        {
            _real = new ReferenceCountedObjectWrapper<IThumbnailData<T>>(real);
            _real.IncrementReferenceCount();
        }

        #region IThumbnailData Members

        public T Image { get { return _real.Item.Image; } }
        object IThumbnailData.Image { get { return _real.Item.Image; } }

        public IThumbnailData Clone()
        {
            _real.IncrementReferenceCount();
            return this;
        }

        #endregion

        protected virtual void DisposeReal()
        {
            _real.Item.Dispose();
            _real = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_real == null)
                return;

            _real.DecrementReferenceCount();
            if (_real.ReferenceCount <= 0)
                DisposeReal();
        }

        #endregion
    }

    public class ThumbnailData<T> : IThumbnailData<T> where T : class, IDisposable
    {
        public ThumbnailData(T image)
        {
            Image = image;
        }

        #region IBitmapThumbnailData Members

        public T Image { get; private set; }

        #endregion

        #region IThumbnailData Members

        object IThumbnailData.Image { get { return Image; } }

        public IThumbnailData Clone()
        {
            if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
                return new ThumbnailData<T>((T) ((ICloneable) Image).Clone());
            
            throw new NotSupportedException();
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (!disposing || Image == null)
                return;

            Image.Dispose();
            Image = null;
        }

        #region IDisposable Members

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
    }
}