#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Input;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers
{
    public interface IMouseElement
    {
        void OnMouseMoving(object sender, MouseEventArgs e);
        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e);
        void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e);
        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e);
        void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e);
        void OnMouseWheeling(object sender, MouseWheelEventArgs e);
        bool HasCapture { get; }
    }
}