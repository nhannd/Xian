#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Windows.Media.Imaging;
using System.Reflection;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Resources;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Views;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Actions
{
    public partial class HelpButton : UserControl, IToolstripButton
    {
        private MouseEvent _mouseEnterEvent;
        private MouseEvent _mouseLeaveEvent;
        private AppServiceReference.WebIconSize _iconSize;

        public HelpButton()
        {
            InitializeComponent();
        }

        #region IToolstripButton Members

        public void RegisterOnMouseEnter(MouseEvent @event)
        {
            _mouseEnterEvent += @event;
        }

        public void RegisterOnMouseLeave(MouseEvent @event)
        {
            _mouseLeaveEvent += @event;
        }

        #endregion

        private void ButtonComponent_Click(object sender, RoutedEventArgs e)
        {
            PopupHelper.PopupContent(DialogTitles.About, new HelpDialogContent());
        }

        private void ButtonComponent_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseLeaveEvent != null)
                _mouseLeaveEvent(this);
        }

        private void ButtonComponent_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseEnterEvent != null)
                _mouseEnterEvent(this);
        }



        #region IToolstripButton Members

        public void SetIconSize(AppServiceReference.WebIconSize iconSize)
        {
            _iconSize = iconSize;
            string assemblyName =  Assembly.GetExecutingAssembly().FullName;
            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));

            switch (_iconSize)
            {
                case AppServiceReference.WebIconSize.Large:

                    HelpIcon.Source =
                        new BitmapImage(
                            new Uri(String.Format("/{0};component/{1}", assemblyName, "Images/HelpLarge.png"),
                                    UriKind.RelativeOrAbsolute));
                    break;

                case AppServiceReference.WebIconSize.Medium:

                    HelpIcon.Source =
                        new BitmapImage(
                            new Uri(String.Format("/{0};component/{1}", assemblyName, "Images/HelpMedium.png"),
                                    UriKind.RelativeOrAbsolute));
                    break;

                case AppServiceReference.WebIconSize.Small:

                    var bmp =
                        new BitmapImage(
                            new Uri(String.Format("/{0};component/{1}", assemblyName, "Images/HelpSmall.png"),
                                    UriKind.RelativeOrAbsolute));
                    HelpIcon.Source = bmp;
                    break;
            }

        }

        #endregion
    }
}
