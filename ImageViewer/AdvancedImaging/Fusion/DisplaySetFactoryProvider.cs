#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layout.Basic;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	//TODO (CR Sept 2010): for as often as we make these providers return more than one object,
	//we should just make the objects themselves the extensions.
	[ExtensionOf(typeof (DisplaySetFactoryProviderExtensionPoint))]
	public class DisplaySetFactoryProvider : IDisplaySetFactoryProvider
	{
		#region IDisplaySetFactoryProvider Members

		public IEnumerable<IDisplaySetFactory> CreateDisplaySetFactories(IPresentationImageFactory presentationImageFactory)
		{
			yield return new PETFusionDisplaySetFactory(PETFusionType.CT);
		}

		#endregion
	}
}