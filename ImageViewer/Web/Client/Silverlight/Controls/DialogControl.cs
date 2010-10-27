#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using ClearCanvas.Web.Client.Silverlight;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls
{
    //TODO: make this implement IPopup and register it with the PopupManager.

    [TemplatePart(Name = "Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "LayoutRoot", Type = typeof(Control))]    
    public class DialogControl : ContentControl, IDisposable
    {
        private static DialogControl _currentDialog;
        private static readonly object _syncLock = new object();

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent", typeof(object), typeof(DialogControl), null);
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(DialogControl), null);
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(DialogControl), null);
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DialogControl), null);
        
        Popup _popup;
        Canvas _backgroundElement;
        FrameworkElement _dialogBoxContainer;

        static public Panel ApplicationRootVisual;
        public event EventHandler Closed;
        public event EventHandler Opened;

        public Brush HeaderForeground
        {
            get { return (Brush)GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public object HeaderContent
        {
            get { return (object)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public DialogControl()
        {
            this.DefaultStyleKey = typeof(DialogControl);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = GetTemplateChild("Popup") as Popup;
            _backgroundElement = GetTemplateChild("BackgroundElement") as Canvas;
            _dialogBoxContainer = GetTemplateChild("DialogBoxContainer") as FrameworkElement;

            _backgroundElement.Opacity = 1;
        }

        public void Show()
        {
            if (_popup!=null && _popup.IsOpen)
                return;

            PopupManager.CloseActivePopup();

            ApplicationRootVisual.Children.Add(this);

            lock (_syncLock)
            {
                if (_currentDialog != null && _currentDialog != this)
                {
                    _currentDialog.Close();
                }

                _currentDialog = this;
            }

            ApplyTemplate();

            UpdateLayout();
            ResizeBackground();
            ResizeDialog();

            _backgroundElement.HorizontalAlignment = HorizontalAlignment.Center;
            _backgroundElement.VerticalAlignment = VerticalAlignment.Center;

            Application.Current.Host.Content.Resized += OnHostResized;

            _popup.IsOpen = true;

            UpdateLayout(); // yes, again to get the desired/actual size of the dialog content
            UpdateClipRect();
            PositionDialog();

            if (Opened != null)
            {
                Opened(this, EventArgs.Empty);
            }
        }

        private void OnHostResized(object sender, EventArgs ev)
        {
            ResizeBackground();
            ResizeDialog();
            UpdateLayout();
            UpdateClipRect();
            PositionDialog();
        }

        private void UpdateClipRect()
        {
            Border dialogBorder = GetTemplateChild("DialogBoxBorder") as Border;
            FrameworkElement contentElement = GetTemplateChild("DialogContent") as FrameworkElement;
            Rect borderInnerBrounds = new Rect
            {
                X = 0,
                Y = 0,
                Width = contentElement.ActualWidth,
                Height = contentElement.ActualHeight
            };

            // make sure the tile and content are clipped
            contentElement.Clip = new RectangleGeometry
            {
                Rect = borderInnerBrounds,
                RadiusX = dialogBorder.CornerRadius.TopLeft - dialogBorder.BorderThickness.Left + 1,
                RadiusY = dialogBorder.CornerRadius.TopLeft - dialogBorder.BorderThickness.Top + 1,
            };
        }

        private void ResizeDialog()
        {
            // Make sure it's not too big
            _dialogBoxContainer.MaxWidth = Application.Current.Host.Content.ActualWidth * 3 / 4;
        }

        public void Close()
        {
            lock (_syncLock)
            {
                if (_currentDialog == this)
                {
                    _currentDialog = null;
                }
            }

            if (_popup != null && _popup.IsOpen)
            {
                _popup.IsOpen = false;
                ApplicationRootVisual.Children.Remove(this);
                if (Closed != null)
                    Closed(this, EventArgs.Empty);
            }
        }


        private void ResizeBackground()
        {
            _backgroundElement.Width = Application.Current.Host.Content.ActualWidth; 
            _backgroundElement.Height = Application.Current.Host.Content.ActualHeight;                
        }

        private void PositionDialog()
        {
            _popup.SetValue(Canvas.TopProperty, Math.Ceiling(_backgroundElement.Height / 2 - _dialogBoxContainer.DesiredSize.Height / 2));
            _popup.SetValue(Canvas.LeftProperty, Math.Ceiling(_backgroundElement.Width / 2 - _dialogBoxContainer.DesiredSize.Width / 2));
        }


        public void Dispose()
        {
            ApplicationRootVisual.Children.Remove(this);
        }

        /// <summary>
        /// Creates a dialog with the given title, message and buttons.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        static public DialogControl Create(string title, string message, IEnumerable<Button> buttons)
        {
            DialogControl dialog = new DialogControl();
            dialog.HeaderContent = new TextBlock { Text = title, FontSize = 14.0d };
            
            StackPanel content = new StackPanel{ Orientation = Orientation.Vertical, Margin = new Thickness(5),};
            content.Children.Add(new TextBlock { Text = message, Margin = new Thickness(5), TextWrapping = TextWrapping.Wrap, FontSize = 14.0d });
            
            if (buttons != null)
            {
                StackPanel buttonPanel = new StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Margin = new Thickness(5)
                };
                foreach (Button button in buttons)
                {
                    button.FontSize = 14.0d;
                    button.Click += (s, ev) => { dialog.Close(); };
                    buttonPanel.Children.Add(button);
                }
                content.Children.Add(buttonPanel);
            }

            dialog.Content = content;
            dialog.Background = new SolidColorBrush(Colors.White);
            dialog.Effect = new DropShadowEffect { Color = Colors.Black };

            dialog.CornerRadius = new CornerRadius(4);
            return dialog;
        }

        /// <summary>
        /// Creates a dialog with the given title, message and buttons.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        static public DialogControl Create(string title, UIElement content, string closeButtonText)
        {
            DialogControl dialog = new DialogControl();
            dialog.HeaderContent = new TextBlock { Text = title, FontSize = 14.0d };

            StackPanel contentPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5), };
            contentPanel.Children.Add(content);

            if (!String.IsNullOrEmpty(closeButtonText))
            {
                StackPanel buttonPanel = new StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Margin = new Thickness(5)
                };

                Button closeButton = new Button { Content = closeButtonText, FontSize = 14.0d };
                closeButton.Click += (s, ev) => { dialog.Close(); };
                buttonPanel.Children.Add(closeButton);

                contentPanel.Children.Add(buttonPanel);
            }           


            dialog.Content = contentPanel;
            dialog.Background = new SolidColorBrush(Colors.White);
            dialog.Effect = new DropShadowEffect { Color = Colors.Black };

            dialog.CornerRadius = new CornerRadius(4);
            return dialog;
        }

        /// <summary>
        /// Displays a dialog with the specified title, message and a close button with the specified text.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="closeButtonText"></param>
        internal static void Show(string title, string message, string closeButtonText)
        {
            DialogControl dialog = Create(title, message, new[] { new Button { Content = closeButtonText } });
            dialog.Show();
        }

        /// <summary>
        /// Displays a dialog with the specified title, message and a close button with the specified text.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="closeButtonText"></param>
        internal static void Show(string title, UIElement content, string closeButtonText)
        {
            DialogControl dialog = Create(title, content, closeButtonText);
            dialog.Show();
        }

        /// <summary>
        /// Pops up a message on the screen. The message will stay until <seealso cref="Close"/> is called.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static DialogControl PopupMessage(string title, string message)
        {
            DialogControl dialog = Create(title, message, null);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Tell if a dialog is currently active on the screen.
        /// </summary>
        internal static bool DialogActive
        {
            get
            {
                lock (_syncLock)
                {
                    return _currentDialog != null;
                }
            }
        }
    }
}
