#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines a pixel data prefetching strategy.
	/// </summary>
	public interface IPrefetchingStrategy
	{
		/// <summary>
		/// Gets the friendly name of the prefetching strategy.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the friendly description of the prefetching strategy.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Starts prefetching pixel data in the background for the images
		/// that have already been added into <paramref name="imageViewer"/>.
		/// </summary>
		void Start(IImageViewer imageViewer);

		/// <summary>
		/// Stops prefetching of pixel data in the background.
		/// </summary>
		/// <remarks>
		/// Implementers should ensure that all background threads have terminated
		/// before this method returns.
		/// </remarks>
		void Stop();
	}
}
