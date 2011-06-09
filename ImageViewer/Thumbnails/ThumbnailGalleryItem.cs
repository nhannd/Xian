using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class ThumbnailGalleryItem : IGalleryItem, INotifyPropertyChanged, IDisposable
    {
        private Image _image;
        private string _name;
        
        internal ThumbnailGalleryItem(object item)
        {
            Platform.CheckForNullReference(item, "item");
            Item = item;
            _name = String.Empty;
        }

        #region Implementation of IGalleryItem

        public Image Image
        {
            get { return _image; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("Thumbnail gallery item has been disposed.");

                if (Equals(_image, value))
                    return;

                if (_image != null)
                    _image.Dispose();

                _image = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("Image"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _name; }
            set
            {
                value = value ?? String.Empty;
                if (value.Equals(_name))
                    return;

                _name = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("Name"));
            }
        }

        public string Description
        {
            get { return String.Empty; }
        }

        public object Item { get; private set; }

        #endregion

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            IsDisposed = true;
            if (Image == null)
                return;

            Image.Dispose();
            Image = null;
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
    }
}