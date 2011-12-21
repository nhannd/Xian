#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class ClearCanvasStyle
	{
		public static Color ClearCanvasDarkBlue
		{
			get
			{
				if (Application.CurrentUITheme != null)
					return Application.CurrentUITheme.Colors.BasicColorDark;
				return Color.FromArgb(61, 152, 209);
			}
		}

		public static Color ClearCanvasBlue
		{
			get
			{
				if (Application.CurrentUITheme != null)
					return Application.CurrentUITheme.Colors.BasicColor;
				return Color.FromArgb(124, 177, 221);
			}
		}

		public static Color ClearCanvasLightBlue
		{
			get
			{
				if (Application.CurrentUITheme != null)
					return Application.CurrentUITheme.Colors.BasicColorLight;
				return Color.FromArgb(186, 210, 236);
			}
		}

		public static void SetTitleBarStyle(TitleBar titleBar)
		{
			titleBar.BackColor = ClearCanvasDarkBlue;
			titleBar.ForeColor = Color.White;
			titleBar.GradientActiveColor = ClearCanvasDarkBlue;
			titleBar.GradientColoring = GradientColoring.LightBackToGradientColor;
			titleBar.GradientDirection = GradientDirection.TopToBottom;
		}
	}
}