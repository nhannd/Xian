#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

		public override IPresentationImage CreateFreshCopy()
		{
			return null;
		}
	}
}

#endif