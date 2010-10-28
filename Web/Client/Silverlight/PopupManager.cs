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
using System.Windows.Browser;

namespace ClearCanvas.Web.Client.Silverlight
{
    public static class PopupManager
    {
        private static IPopup _activePopup;

        static PopupManager()
        {
        }

        internal static IPopup ActivePopup
        {
            get { return _activePopup; }
            set
            {
                if (ReferenceEquals(value, _activePopup))
                    return;

                var old = _activePopup;
                _activePopup = value;
                if (old != null)
                    old.IsOpen = false;
            }
        }

        public static void CloseActivePopup()
        {
            if (_activePopup != null)
                _activePopup.IsOpen = false;
        }

        internal static void RegisterSingletonPopup(IPopup popup)
        {
            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;
        }

        private static void OnPopupOpened(object sender, EventArgs e)
        {
            ActivePopup = (IPopup)sender;
        }

        private static void OnPopupClosed(object sender, EventArgs e)
        {
            var popup = (IPopup)sender;
            if (ReferenceEquals(_activePopup, popup))
                ActivePopup = null;
        }
   }
}