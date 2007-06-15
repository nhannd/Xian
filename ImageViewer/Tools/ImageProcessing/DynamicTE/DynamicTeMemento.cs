using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTeMemento : IMemento
	{
		private double _te;

		public DynamicTeMemento(double te)
		{
			_te = te;
		}

		public double Te
		{
			get { return _te; }
		}
	}
}
