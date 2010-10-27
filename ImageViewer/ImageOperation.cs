#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Abstract base class for an undoable operation performed on an <see cref="IPresentationImage"/>.
	/// </summary>
	public abstract class ImageOperation : UndoableOperation<IPresentationImage>
	{
		/// <summary>
		/// Default protected constructor.
		/// </summary>
		protected ImageOperation()
		{
		}
	}
}
