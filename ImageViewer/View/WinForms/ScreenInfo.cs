using System.Windows.Forms;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	internal class ScreenInfo : IScreenInfo
	{
		private readonly string _name;
		private readonly int _width;
		private readonly int _height;
		private readonly int _bitDepth;

		public ScreenInfo(Control control)
		{
			Screen screen = Screen.FromControl(control);
			_name = screen.DeviceName;
			_width = screen.Bounds.Width;
			_height = screen.Bounds.Height;
			_bitDepth = screen.BitsPerPixel;
		}

		public string Name
		{
			get { return _name;  }	
		}

		public int Width
		{
			get { return _width; }
		}
		
		public int Height
		{
			get { return _height; }
		}
		
		public int BitDepth
		{
			get { return _bitDepth; }
		}
	}
}
