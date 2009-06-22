#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
