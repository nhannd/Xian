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
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public static class EventBroker
    {
        /// <summary>
        /// Occurs when a tile "HasCapture" property is changed.
        /// </summary>
        public static event EventHandler TileHasCaptureChanged;

        public static void OnTileHasCaptureChanged(Tile tileEntity)
        {
            if (TileHasCaptureChanged != null)
                TileHasCaptureChanged(tileEntity, EventArgs.Empty);
        }
    }
}
