using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailGalleryItem : IGalleryItem, IDisposable
    {
        IDisplaySet DisplaySet { get; }
    }

    public class ThumbnailGalleryItem : IThumbnailGalleryItem
    {
        private Image _image;

        public ThumbnailGalleryItem(ThumbnailDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public ThumbnailDescriptor Descriptor { get; set; }

        #region Implementation of IThumbnailGalleryItem

        public IDisplaySet DisplaySet
        {
            get { return Descriptor.DisplaySet; }
        }

        #endregion

        #region Implementation of IGalleryItem

        public Image Image
        {
            get { return _image; }
            set
            {
                if (Equals(_image, value))
                    return;

                if (_image != null)
                    _image.Dispose();

                _image = value;
            }
        }

        public virtual string Name
        {
            get
            {
                string name = DisplaySet.Name;
                name = name.Replace("\r\n", " ");
                name = name.Replace("\r", " ");
                name = name.Replace("\n", " ");

                int number = DisplaySet.PresentationImages.Count;
                if (number <= 1)
                    return String.Format(SR.FormatThumbnailName1Image, name);

                return String.Format(SR.FormatThumbnailName, number, name);
            }
            set
            {
                throw new NotSupportedException("The name of thumbnails cannot be changed.");
            }
        }

        public string Description
        {
            get { return String.Empty; }
        }

        public object Item
        {
            get { return DisplaySet; }
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

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