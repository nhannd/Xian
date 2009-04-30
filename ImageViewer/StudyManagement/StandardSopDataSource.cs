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
	public abstract class StandardSopDataSource : SopDataSource
	{
		//I hate doing this, but it's horribly inefficient for all subclasses to do their own locking.
		protected readonly object SyncLock = new object();
		private volatile WeakReference[] _framePixelData;

		protected StandardSopDataSource()
			: base()
		{
		}

		private WeakReference[] FramePixelData
		{
			get
			{
				if (_framePixelData == null)
				{
					lock(SyncLock)
					{
						if (_framePixelData == null)
						{
							_framePixelData = new WeakReference[NumberOfFrames];
							for (int i = 0; i < _framePixelData.Length; ++i)
								_framePixelData[i] = new WeakReference(null);
						}
					}
				}

				return _framePixelData;
			}
		}

		protected abstract byte[] CreateFrameNormalizedPixelData(int frameNumber);

		protected sealed override void OnGetFrameNormalizedPixelData(int frameNumber, out byte[] pixelData)
		{
			int frameIndex = frameNumber - 1;
			WeakReference reference = FramePixelData[frameIndex];

			try
			{
				pixelData = reference.Target as byte[];
			}
			catch(InvalidOperationException)
			{
				pixelData = null;
				reference = new WeakReference(null);
				FramePixelData[frameIndex] = reference;
			}

			if (!reference.IsAlive || pixelData == null)
			{
				lock(SyncLock)
				{
					pixelData = CreateFrameNormalizedPixelData(frameNumber);
					reference.Target = pixelData;
				}
			}
		}

		
		//NOTE: no need to implement anything here, at least for pixel data, since we're using a WeakReferenceCache.
		protected virtual void OnUnloadFrameData(int frameNumber)
		{
		}

		public sealed override void UnloadFrameData(int frameNumber)
		{
			lock(SyncLock)
			{
				OnUnloadFrameData(frameNumber);
			}
		}
	}
}
