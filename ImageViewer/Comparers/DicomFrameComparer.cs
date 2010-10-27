#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparing <see cref="Frame"/>s.
	/// </summary>
	public abstract class DicomFrameComparer : PresentationImageComparer, IComparer<Frame>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomFrameComparer"/>.
		/// </summary>
		protected DicomFrameComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomFrameComparer"/>.
		/// </summary>
		protected DicomFrameComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		public override int Compare(IPresentationImage x, IPresentationImage y)
		{
			IImageSopProvider xProvider = x as IImageSopProvider;
			IImageSopProvider yProvider = y as IImageSopProvider;

			if (ReferenceEquals(xProvider, yProvider))
				return 0; //same object or both are null

			//at this point, at least one of x or y is non-null and they are not the same object

			if (xProvider == null)
				return -ReturnValue; // x > y (because we want x at the end for non-reverse sorting)
			if (yProvider == null)
				return ReturnValue; // x < y (because we want y at the end for non-reverse sorting)

			if (ReferenceEquals(xProvider.Frame, yProvider.Frame))
				return 0;

			return Compare(xProvider.Frame, yProvider.Frame);
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="Frame"/>s.
		/// </summary>
		public abstract int Compare(Frame x, Frame y);
	}
}
