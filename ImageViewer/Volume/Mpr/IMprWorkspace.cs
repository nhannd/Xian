#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Defines a container for <see cref="IImageSet"/> objects in an <see cref="MprViewerComponent"/>.
	/// </summary>
	public interface IMprWorkspace : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="MprViewerComponent"/>.
		/// </summary>
		MprViewerComponent MprViewer { get; }

		/// <summary>
		/// Gets a collection of <see cref="IImageSet"/> objects that belong to
		/// this <see cref="IMprWorkspace"/>.
		/// </summary>
		ObservableList<IImageSet> ImageSets { get; }
	}
}