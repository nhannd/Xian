#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    //NOTE: could move the generic part(s) of this down to Desktop later.

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

        public IImageData ImageData
        {
            get { return _imageData; }
            set
            {
                if (Equals(_imageData, value))
                    return;

                var old = _imageData;
                _imageData = value;
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("Image"));
                EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs("ImageData"));

                if (old != null)
                    old.Dispose();
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

        protected virtual void Dispose(bool disposing)
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