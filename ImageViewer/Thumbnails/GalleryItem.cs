using System;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public class GalleryItem : IGalleryItem
    {
        private IImageData _imageData;
        private string _name;
        private string _description;

        internal GalleryItem(object item)
        {
            Platform.CheckForNullReference(item, "item");
            Item = item;
            _name = String.Empty;
            _description = String.Empty;
        }

        public virtual IImageData ImageData
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
            get { return ImageData == null ? null : ImageData.Image; }
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
            get { return _description; }
            set
            {
                value = value ?? String.Empty;
                if (value == _name)
                    return;

                _description = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("Description"));
            }
        }

        public object Item { get; private set; }

        #endregion

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