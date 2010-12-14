#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers
{
    public class PopupHelper
    {
        public static ChildWindow PopupContent(string title, object content)
        {
            ChildWindow msgBox = new ChildWindow();
            msgBox.Style = System.Windows.Application.Current.Resources["PopupMessageWindow"] as Style;
            msgBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xBA, 0xD2, 0xEC));
            msgBox.Title = title;

            msgBox.MaxWidth = Application.Current.Host.Content.ActualWidth * 0.75;

            StackPanel panel = new StackPanel();
            panel.Children.Add(new ContentPresenter() { Content = content });

            var closeButton = new Button { Content = "Close", HorizontalAlignment = HorizontalAlignment.Center };
            closeButton.Click += (s, e) => { msgBox.Close(); };
            panel.Children.Add(closeButton);

            msgBox.Content = panel;

            msgBox.Show();
            return msgBox;
        }

        public static ChildWindow PopupMessage( string title, string message)
        {
            ChildWindow msgBox = new ChildWindow();
            msgBox.Style = System.Windows.Application.Current.Resources["PopupMessageWindow"] as Style;
            msgBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xBA, 0xD2, 0xEC));
            msgBox.Title = title;

            msgBox.MaxWidth = Application.Current.Host.Content.ActualWidth * 0.75; 
            
            msgBox.Content = new TextBlock() { Text = message, Margin = new Thickness(20), Foreground = new SolidColorBrush(Colors.White), FontSize = 14 };
            msgBox.Show();
            return msgBox;
        }

        public static ChildWindow PopupMessage(string title, string message, string closeButtonLabel)
        {
            ChildWindow msgBox = new ChildWindow();
            msgBox.Style = System.Windows.Application.Current.Resources["PopupMessageWindow"] as Style;
            msgBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xBA, 0xD2, 0xEC));
            msgBox.Title = title;
            msgBox.MaxWidth = Application.Current.Host.Content.ActualWidth * 0.75;

            StackPanel content = new StackPanel();
            content.Children.Add(new TextBlock() { Text = message, Margin = new Thickness(20), Foreground = new SolidColorBrush(Colors.White), FontSize = 14, HorizontalAlignment=HorizontalAlignment.Center });

            Button closeButton = new Button() { Content = closeButtonLabel, FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center };
            closeButton.Click += (s, o) => { msgBox.Close(); };
            content.Children.Add(closeButton);
            msgBox.Content = content;
            msgBox.Show();
            return msgBox;
        }
    }
}
