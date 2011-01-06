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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	public class ImagePropertyProviderExtensionPoint : ExtensionPoint<IImagePropertyProvider>
	{ }

	//TODO (cr Oct 2009): Properties window could be used for graphics, too.  In fact, it could be totally general.
	public interface IImagePropertyProvider
	{
		IImageProperty[] GetProperties(IPresentationImage image);
	}
}