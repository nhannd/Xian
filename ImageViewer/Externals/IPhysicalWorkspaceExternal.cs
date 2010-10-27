#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Externals
{
	public interface IPresentationImageExternal : IExternal
	{
		bool CanLaunch(IPresentationImage image);
		bool Launch(IPresentationImage image);
	}

	public interface IDisplaySetExternal : IExternal
	{
		bool CanLaunch(IEnumerable<IPresentationImage> images);
		bool Launch(IEnumerable<IPresentationImage> images);

		bool CanLaunch(IDisplaySet displaySet);
		bool Launch(IDisplaySet displaySet);
	}

	public interface IImageSetExternal : IExternal
	{
		bool CanLaunch(IEnumerable<IDisplaySet> displaySets);
		bool Launch(IEnumerable<IDisplaySet> displaySets);

		bool CanLaunch(IImageSet imageSet);
		bool Launch(IImageSet imageSet);
	}

	public interface IPhysicalWorkspaceExternal : IExternal
	{
		bool CanLaunch(IEnumerable<IImageSet> imageSets);
		bool Launch(IEnumerable<IImageSet> imageSets);

		bool CanLaunch(IPhysicalWorkspace physicalWorkspace);
		bool Launch(IPhysicalWorkspace physicalWorkspace);
	}
}