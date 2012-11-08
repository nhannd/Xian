#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    internal delegate IThumbnailLoader GetThumbnailLoader();

    public class ThumbnailGallery : Gallery<IDisplaySet>, IDisposable
    {
        private ThumbnailLoader _loader;
        private IThumbnailFactory<IPresentationImage> _thumbnailFactory;
        private Size _thumbnailSize;
        private bool _isVisible;


        public ThumbnailGallery()
            : this(new BindingList<IGalleryItem>())
        {
        }

        public ThumbnailGallery(bool suppressLoadingThumbnails)
            : this(new BindingList<IGalleryItem>(), suppressLoadingThumbnails)
        {
        }

        public ThumbnailGallery(IEnumerable<IGalleryItem> galleryItems)
            : this(galleryItems, false)
        {
        }

        public ThumbnailGallery(IEnumerable<IGalleryItem> galleryItems, bool suppressLoadingThumbnails)
            : base(galleryItems)
        {
            base.GalleryItemFactory = new ThumbnailGalleryItemFactory(() => ThumbnailLoader, suppressLoadingThumbnails);
            _thumbnailSize = ThumbnailSizes.Medium;
        }

        private IThumbnailLoader ThumbnailLoader
        {
            get
            {
                if (_loader == null)
                    _loader = new ThumbnailLoader(new NullThumbnailRepository(ThumbnailFactory));
                return _loader;
            }
        }

        private IThumbnailFactory<IPresentationImage> ThumbnailFactory
        {
            get
            {
                if (_thumbnailFactory == null)
                    _thumbnailFactory = new ThumbnailFactory();
                return _thumbnailFactory;
            }
        }

        public Size ThumbnailSize
        {
            get { return _thumbnailSize; }
            set
            {
                if (value == _thumbnailSize)
                    return;

                _thumbnailSize = value;
                foreach (IThumbnailGalleryItem galleryItem in GalleryItems)
                    galleryItem.ThumbnailSize = _thumbnailSize;
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible)
                    return;

                _isVisible = value;
                foreach (IThumbnailGalleryItem galleryItem in GalleryItems)
                    galleryItem.IsVisible = _isVisible;
            }
        }

        public void SetThumbnailFactory(IThumbnailFactory<IPresentationImage> thumbnailFactory)
        {
            Platform.CheckForNullReference(thumbnailFactory, "thumbnailFactory");
            _thumbnailFactory = thumbnailFactory;
        }

        public void SetBitmapConverter(BitmapConverter bitmapConverter)
        {
            SetThumbnailFactory(new ThumbnailFactory(bitmapConverter));
        }

        protected override IGalleryItem CreateNew(IDisplaySet item)
        {
            var newItem = (IThumbnailGalleryItem)base.CreateNew(item);
            newItem.ThumbnailSize = _thumbnailSize;
            newItem.IsVisible = IsVisible;
            return newItem;
        }
    }
}