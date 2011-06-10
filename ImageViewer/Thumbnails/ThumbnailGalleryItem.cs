using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailGalleryItem : IGalleryItem, INotifyPropertyChanged, IDisposable
    {
        bool IsVisible { get; set; }
        IThumbnailData ImageData { get; }
    }

    public class ThumbnailGalleryItem : IThumbnailGalleryItem
    {
        private IThumbnailData _imageData;
        private string _name;
        private bool _isVisible;

        internal ThumbnailGalleryItem(object item)
        {
            Platform.CheckForNullReference(item, "item");
            Item = item;
            _name = String.Empty;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible)
                    return;

                _isVisible = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("IsVisible"));
            }
        }

        public IThumbnailData ImageData
        {
            get { return _imageData; }
            set
            {
                if (Equals(_imageData, value))
                    return;

                if (_imageData != null)
                    _imageData.Dispose();

                _imageData = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("ImageData"));
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("Image"));
            }
        }

        #region Implementation of IGalleryItem

        public object Image
        {
            get { return _imageData.Image; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _name; }
            set
            {
                value = value ?? String.Empty;
                if (value == _name)
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

        public object State { get; set; }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_imageData == null)
                return;

            _imageData.Dispose();
            _imageData = null;
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