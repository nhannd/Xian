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

		public IComposableLut GetLut(IPresentationImage presentationImage)
		{
			// TODO: Eventually, this should use the IPresetVoiLutOperationFactory extensions and simply
			// try to apply each one that matches in order until one works.  The 'Auto' lut operation would
			// be implemented as an operation (with a corresponding factory) and treated just like the rest of the presets.
			// However, right now we don't want to add new functionality to 1.0, so the 'Initial Lut Provider' and the
			// 'Auto Lut Operation' do basically the same thing.

			return AutoPresetVoiLutOperationComponent.GetInitialLut(presentationImage);
		}

		#endregion
	}
}