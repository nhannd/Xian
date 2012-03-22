using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	class ProgressBarIconSet : IconSet
	{
		private readonly Size _dimensions;
		private readonly decimal _percent;

		public ProgressBarIconSet(string name, Size dimensions, decimal percent)
			: base(name)
		{
			_dimensions = dimensions;
			_percent = percent;
		}

		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			var bitmap = new Bitmap(_dimensions.Width, _dimensions.Height);
			using(var g = System.Drawing.Graphics.FromImage(bitmap))
			{
				g.DrawRectangle(Pens.DarkGray, 0, 0, _dimensions.Width - 1, _dimensions.Height - 1);
				g.FillRectangle(Brushes.GreenYellow, 1, 1,
					Math.Min((int)(_dimensions.Width*_percent/100), _dimensions.Width - 2),
					_dimensions.Height - 2);
			}
			return bitmap;
		}
	}
}
