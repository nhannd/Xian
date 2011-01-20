#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Used to apply an <see cref="IUndoableOperation{T}">undoable operation</see> to an <see cref="IPresentationImage"/>.
	/// </summary>
	[Obsolete("This interface is now obsolete.  Use IUndoableOperation<IPresentationImage> instead.")]
	public interface IImageOperation : IUndoableOperation<IPresentationImage>
	{
	}
}
