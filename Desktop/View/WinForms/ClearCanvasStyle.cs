using System.Drawing;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class ClearCanvasStyle
	{
		public static Color ClearCanvasDarkBlue
		{
			get { return Color.FromArgb(61, 152, 209); }
		}

		public static Color ClearCanvasBlue
		{
			get { return Color.FromArgb(124, 177, 221); }
		}

		public static Color ClearCanvasLightBlue
		{
			get { return Color.FromArgb(186, 210, 236); }
		}

		public static void SetTitleBarStyle(TitleBar titleBar)
		{
			titleBar.BackColor = ClearCanvasDarkBlue;
			titleBar.ForeColor = Color.White;
			titleBar.GradientActiveColor = ClearCanvasDarkBlue;
			titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToGradientColor;
			titleBar.GradientDirection = Crownwood.DotNetMagic.Controls.GradientDirection.TopToBottom;
		}
	}
}
