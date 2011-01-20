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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Controls.Primitives;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using ClearCanvas.Web.Client.Silverlight;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class ImageBoxScrollbarView : UserControl, IDisposable
    {
        //TODO (CR May 2010): c-style naming
		public const string IMAGE_COUNT_PROPERTY_NAME = "ImageCount";
        public const string TILES_PROPERTY_NAME = "Tiles";
        public const string TOP_LEFT_PRESENTATION_IMAGE_INDEX_PROPERTY_NAME = "TopLeftPresentationImageIndex";
        
        private ImageBox ServerEntity { get; set; }
        private DelayedEventPublisher<ScrollBarUpdateEventArgs> _scrollbarEventPublisher;

        public ImageBoxScrollbarView(ImageBox imageBox)
        {
            IsTabStop = true; // allow focus
            ServerEntity = imageBox;

            DataContext = imageBox; 
            
            InitializeComponent();

            LayoutRoot.IsHitTestVisible = !imageBox.Tiles.Any(t => t.HasCapture);

            EventBroker.TileHasCaptureChanged += new EventHandler(EventBroker_TileHasCaptureChanged);

            ImageScrollBar.SetBinding(System.Windows.Controls.Primitives.ScrollBar.ValueProperty,
                    new Binding(TOP_LEFT_PRESENTATION_IMAGE_INDEX_PROPERTY_NAME) { 
                        Mode = BindingMode.OneTime 
            });

            ImageScrollBar.Maximum = ServerEntity.ImageCount - ServerEntity.Tiles.Count;
            ImageScrollBar.Visibility = ImageScrollBar.Maximum > 0 ? Visibility.Visible : Visibility.Collapsed;

            ServerEntity.PropertyChanged += OnPropertyChanged;

            _scrollbarEventPublisher = new DelayedEventPublisher<ScrollBarUpdateEventArgs>((s, ev) =>
            {
                ApplicationContext.Current.ServerEventBroker.DispatchMessage(new UpdatePropertyMessage()
                {
                    Identifier = Guid.NewGuid(),
                    PropertyName = TOP_LEFT_PRESENTATION_IMAGE_INDEX_PROPERTY_NAME,
                    TargetId = ServerEntity.Identifier,
                    Value = ev.ScrollbarPosition
                });
            }, 100);
        }



        void EventBroker_TileHasCaptureChanged(object sender, EventArgs e)
        {
            LayoutRoot.IsHitTestVisible = (MouseHelper.ActiveElement == null || !MouseHelper.ActiveElement.HasCapture);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs ev)
        {
            if (ev.PropertyName == TOP_LEFT_PRESENTATION_IMAGE_INDEX_PROPERTY_NAME)
            {
                ImageScrollBar.Value = ServerEntity.TopLeftPresentationImageIndex;
            }
            else if (ev.PropertyName == IMAGE_COUNT_PROPERTY_NAME)
            {
                ImageScrollBar.Maximum = ServerEntity.ImageCount - ServerEntity.Tiles.Count;
            }
            else if (ev.PropertyName == TILES_PROPERTY_NAME)
            {
                ImageScrollBar.Maximum = ServerEntity.ImageCount - ServerEntity.Tiles.Count;
            }

            ImageScrollBar.Visibility = ImageScrollBar.Maximum > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ImageScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            PopupManager.CloseActivePopup();
            Focus();
            switch (e.ScrollEventType)
            {
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.SmallDecrement:
                case ScrollEventType.SmallIncrement:
                case ScrollEventType.Last:
                case ScrollEventType.LargeIncrement:
                case ScrollEventType.LargeDecrement:
                case ScrollEventType.First:
                    _scrollbarEventPublisher.Publish(sender, new ScrollBarUpdateEventArgs { ScrollbarPosition = (int)e.NewValue });
                    
                    // don't move the thumb, keep in sync with the server side.
                    // Update will be done via PropertyChange event
                    ImageScrollBar.Value = ServerEntity.TopLeftPresentationImageIndex;
                    break;

                case ScrollEventType.EndScroll: 
                    // ignore it
                    break;
            }
        }

        public void Dispose()
        {
            if (_scrollbarEventPublisher != null)
            {
                _scrollbarEventPublisher.Dispose();
                _scrollbarEventPublisher = null;
            }

            EventBroker.TileHasCaptureChanged -= EventBroker_TileHasCaptureChanged;
        }
    }

    class ScrollBarUpdateEventArgs : EventArgs
    {
        public int ScrollbarPosition { get; set; }
    }

}
