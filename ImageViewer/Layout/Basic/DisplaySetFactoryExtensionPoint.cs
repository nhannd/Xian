#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExtensionPoint]
	public sealed class DisplaySetFactoryProviderExtensionPoint : ExtensionPoint<IDisplaySetFactoryProvider> {}

	public interface IDisplaySetFactoryProvider
	{
		IEnumerable<IDisplaySetFactory> CreateDisplaySetFactories(IPresentationImageFactory presentationImageFactory);
	}
}