#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

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

#endif