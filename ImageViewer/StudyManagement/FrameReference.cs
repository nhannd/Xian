#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Interface defining a transient reference to a <see cref="Frame"/>.
	/// </summary>
	/// <remarks>
	/// See <see cref="ISopReference"/> for a detailed explanation of 'transient references'.
	/// </remarks>
	public interface IFrameReference : IImageSopProvider, IDisposable
	{
		/// <summary>
		/// Clones an existing <see cref="IFrameReference"/>, creating a new transient reference.
		/// </summary>
		IFrameReference Clone();
	}

	public partial class Frame
	{
		private class FrameReference : IFrameReference
		{
			private readonly Frame _frame;
			private ISopReference _sopReference;

			public FrameReference(Frame frame)
			{
				_frame = frame;
				_sopReference = _frame.ParentImageSop.CreateTransientReference();
			}

			#region IFrameReference Members

			public IFrameReference Clone()
			{
				return new FrameReference(_frame);
			}

			#endregion

			#region IImageSopProvider Members

			public ImageSop ImageSop
			{
				get { return _frame.ParentImageSop; }
			}

			public Frame Frame
			{
				get { return _frame; }
			}

			#endregion

			#region ISopProvider Members
			
			Sop ISopProvider.Sop
			{
				get { return _frame.ParentImageSop; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_sopReference != null)
				{
					_sopReference.Dispose();
					_sopReference = null;
				}
			}

			#endregion
		}
	}
}
