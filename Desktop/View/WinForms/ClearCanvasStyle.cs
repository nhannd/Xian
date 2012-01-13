#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Helper class for initialization of controls to the ClearCanvas visual style, now deprecated in favour of extensible application GUI themes.
	/// </summary>
	[Obsolete("The use of this class has been deprecated in favour of extensible application GUI themes")]
	public static class ClearCanvasStyle
	{
		/// <summary>
		/// Deprecated. Use <see cref="Application.CurrentUITheme"/> to retrieve the current colour scheme instead.
		/// </summary>
		[Obsolete("The use of the hard coded color scheme has been deprecated in favour of extensible application GUI themes")]
		public static Color ClearCanvasDarkBlue
		{
			get { return Application.CurrentUITheme.Colors.StandardColorDark; }
		}

		/// <summary>
		/// Deprecated. Use <see cref="Application.CurrentUITheme"/> to retrieve the current colour scheme instead.
		/// </summary>
		[Obsolete("The use of the hard coded color scheme has been deprecated in favour of extensible application GUI themes")]
		public static Color ClearCanvasBlue
		{
			get { return Application.CurrentUITheme.Colors.StandardColorBase; }
		}

		/// <summary>
		/// Deprecated. Use <see cref="Application.CurrentUITheme"/> to retrieve the current colour scheme instead.
		/// </summary>
		[Obsolete("The use of the hard coded color scheme has been deprecated in favour of extensible application GUI themes")]
		public static Color ClearCanvasLightBlue
		{
			get { return Application.CurrentUITheme.Colors.StandardColorLight; }
		}

		/// <summary>
		/// Deprecated. Use the <see cref="TitleBar"/> control instead, which will automatically take into account the value of <see cref="Application.CurrentUITheme"/>.
		/// </summary>
		[Obsolete("The use of the toolkit title bar control has been deprecated in favour of using ClearCanvas.Desktop.View.WinForms.TitleBar")]
		public static void SetTitleBarStyle(Crownwood.DotNetMagic.Controls.TitleBar titleBar)
		{
			titleBar.BackColor = ClearCanvasDarkBlue;
			titleBar.ForeColor = Color.White;
			titleBar.GradientActiveColor = ClearCanvasDarkBlue;
			titleBar.GradientColoring = GradientColoring.LightBackToGradientColor;
			titleBar.GradientDirection = GradientDirection.TopToBottom;
		}
	}
}