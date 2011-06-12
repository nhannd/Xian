#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IImageData<T> : IImageData
    {
        new T Image { get; }
        new IImageData<T> Clone();
    }

    public interface IImageData : IDisposable
    {
        object Image { get; }
        IImageData Clone();
    }

    public interface IImageDataFactory
    {
        IImageData CreateImage(object sourceItem);
    }

    public interface IImageDataFactory<TSourceItem>
    {
        IImageData CreateImage(TSourceItem sourceItem);
    }
    
    public class MemoryStreamImageData : ImageDataBase<MemoryStream>
    {
        public MemoryStreamImageData(MemoryStream image)
            : base(image)
        {
        }

        public override IImageData<MemoryStream> Clone()
        {
            return new MemoryStreamImageData(new MemoryStream(Image.GetBuffer()));
        }
    }

    public class CloneableImageData<T> : ImageDataBase<T> where T : class, IDisposable, ICloneable
    {
        public CloneableImageData(T image)
            : base(image)
        {
        }

        public override IImageData<T> Clone()
        {
            return new CloneableImageData<T>((T)Image.Clone());
        }
    }
    
    public abstract class ImageDataBase<T> : IImageData<T> where T : class, IDisposable
    {
        protected ImageDataBase(T image)
        {
            Image = image;
        }

        #region IImageData<T> Members

        public T Image { get; private set; }
        public abstract IImageData<T> Clone();

        #endregion

        #region IImageData Members

        object IImageData.Image { get { return Image; } }
        IImageData IImageData.Clone()
        {
            return Clone();
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

    /*
    internal class ImageDataProxy<T> : IImageData<T> where T : class
    {
        private ReferenceCountedObjectWrapper<IImageData<T>> _real;

        public ImageDataProxy(IImageData<T> real)
        {
            _real = new ReferenceCountedObjectWrapper<IImageData<T>>(real);
            _real.IncrementReferenceCount();
        }

        #region IImageData<T> Members

        public T Image { get { return _real.Item.Image; } }
        public IImageData<T> Clone()
        {
            _real.IncrementReferenceCount();
            return this;
        }

        #endregion

        #region IImageData<T> Members

        object IImageData.Image { get { return _real.Item.Image; } }

        IImageData IImageData.Clone()
        {
            return Clone();
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
    */
}