using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Tests
{
	public class MockPresentationImage : PresentationImage
	{
		public override ClearCanvas.ImageViewer.Rendering.IRenderer ImageRenderer
		{
			get { return null; }
		}
	}
}
