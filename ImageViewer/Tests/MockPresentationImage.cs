using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Tests
{
	public class MockPresentationImage : PresentationImage
	{
		public override IRenderer ImageRenderer
		{
			get { return null; }
		}

		public override IPresentationImage Clone()
		{
			return null;
		}
	}
}
