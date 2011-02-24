#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    class Item
    {
        public Guid Key;
        public BitmapSource BitmapSource;
    }
    public class TileImageHistory
    {
        static TileImageHistory _instance = new TileImageHistory();

        List<Item> _items = new List<Item>();
        private TileImageHistory() 
        { 
            
        }

        static public TileImageHistory Instance
        {
            get { return _instance; }
        }

        public UIElement GUI
        {
            get 
            {
                var panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                foreach (Item item in _items)
                {
                    var section = new StackPanel { Orientation = Orientation.Vertical, Margin=new Thickness(5) };
                    section.Children.Add(new TextBlock { Text = String.Format("Key={0}", item.Key), TextAlignment = System.Windows.TextAlignment.Center });

                    var image = new Image { Source = item.BitmapSource, Margin = new Thickness(5) };
                    section.Children.Add(image);

                    panel.Children.Add(section);
                }
                return panel; 
            }
        }
        public void Add(Guid key, BitmapSource bmp)
        {
            _items.Add(new Item { Key = key, BitmapSource = bmp });

            if (_items.Count > 10)
            {
                _items.RemoveAt(0);
            }
        }

    }
}
