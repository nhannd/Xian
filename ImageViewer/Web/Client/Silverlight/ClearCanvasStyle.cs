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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    // TODO: Get rid of this. It makes it harder to change. Use resource dictionary instead
	public static class ClearCanvasStyle
	{
		public static Color ClearCanvasDarkBlue
		{
			get { return Color.FromArgb(255, 61, 152, 209); }
		}

		public static Color ClearCanvasBlue
		{
			get { return Color.FromArgb(255, 124, 177, 221); }
		}

		public static Color ClearCanvasLightBlue
		{
			get { return Color.FromArgb(255, 186, 210, 236); }
		}

        public static Color ClearCanvasCheckedButtonGlow
        {
            get { return Color.FromArgb(255, 246, 200, 0); }
        }

        public static Color ClearCanvasButtonOutlineChecked
        {
            get { return ClearCanvasStyle.GetPredefinedColor("Yellow"); }
        }

        public static Color ClearCanvasButtonOutlineUnchecked
        {
            get { return Color.FromArgb(255, 60, 60, 60); }
        }

        public static Color GetPredefinedColor(string color)
        {
            string xamlString = "<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Background=\"" + color + "\"/>";
            try
            {
                Canvas c = (Canvas)System.Windows.Markup.XamlReader.Load(xamlString);
                SolidColorBrush colorBrush = (SolidColorBrush)c.Background;
                return colorBrush.Color;
            }
            catch (Exception)
            {
                //If an invalid string is passed in, return red, which should make it obvious that the color wasn't set properly..
                return Colors.Red;
            }
        }
	}
}
