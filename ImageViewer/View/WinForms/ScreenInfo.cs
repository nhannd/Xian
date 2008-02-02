using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	/// <summary>
	/// A warpper for the WinForms Screen class
	/// </summary>
	internal class ScreenInfo : IScreenInfo
	{
		private readonly Screen _screen;

		public ScreenInfo(Screen screen)
		{
			_screen = screen;
		}

		public int BitsPerPixel
		{
			get { return _screen.BitsPerPixel; }
		}

		public Rectangle Bounds
		{
			get { return _screen.Bounds; }
		}

		public string DeviceName
		{
			get { return _screen.DeviceName; }
		}

		public bool Primary
		{
			get { return _screen.Primary; }
		}

		public Rectangle WorkingArea
		{
			get { return _screen.WorkingArea; }
		}
	}
}
