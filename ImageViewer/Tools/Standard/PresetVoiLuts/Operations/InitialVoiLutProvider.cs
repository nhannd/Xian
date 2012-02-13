#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	[ExtensionOf(typeof(InitialVoiLutProviderExtensionPoint))]
	public sealed class InitialVoiLutProvider : IInitialVoiLutProvider
	{
		public InitialVoiLutProvider()
		{
		}

		#region IInitialVoiLutProvider Members

		public IVoiLut GetLut(IPresentationImage presentationImage)
		{
            var applicator = AutoVoiLutApplicator.Create(presentationImage);
            return applicator == null ? null : applicator.GetInitialLut();
        }

		#endregion
	}
}