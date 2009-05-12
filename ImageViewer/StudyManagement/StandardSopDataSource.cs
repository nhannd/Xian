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
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class StandardSopDataSource : SopDataSource
	{
		//I hate doing this, but it's horribly inefficient for all subclasses to do their own locking.
		protected readonly object SyncLock = new object();

		private ISopFrameData[] _frameData;

		protected StandardSopDataSource() : base() {}

		protected abstract StandardSopFrameData CreateFrameData(int frameNumber);

		protected sealed override ISopFrameData OnGetFrameData(int frameNumber)
		{
			if(_frameData == null)
			{
				lock(this.SyncLock)
				{
					if(_frameData == null)
					{
						_frameData = new ISopFrameData[this.NumberOfFrames];
						for (int n = 0; n < _frameData.Length; n++)
							_frameData[n] = this.CreateFrameData(n + 1);
					}
				}
			}
			return _frameData[frameNumber - 1];
		}

		protected abstract class StandardSopFrameData : SopFrameData
		{
			private readonly Dictionary<int, WeakReference> _overlayData = new Dictionary<int, WeakReference>();
			private WeakReference _pixelData = new WeakReference(null);

			public StandardSopFrameData(int frameNumber, StandardSopDataSource parent) : base(frameNumber, parent) {}

			public new StandardSopDataSource Parent
			{
				get { return (StandardSopDataSource) base.Parent; }
			}

			public sealed override byte[] GetNormalizedPixelData()
			{
				byte[] data;

				try
				{
					data = _pixelData.Target as byte[];
				}
				catch(InvalidOperationException)
				{
					_pixelData = new WeakReference(data = null);
				}

				if(!_pixelData.IsAlive || data == null)
				{
					lock(this.Parent.SyncLock)
					{
						_pixelData.Target = data = CreateNormalizedPixelData();
					}
				}

				return data;
			}

			protected abstract byte[] CreateNormalizedPixelData();

			public override sealed byte[] GetNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
			{
				if(overlayGroupNumber < 1)
					throw new ArgumentOutOfRangeException("overlayGroupNumber", overlayGroupNumber, "Must be a positive, non-zero number.");
				if (overlayFrameNumber < 1)
					throw new ArgumentOutOfRangeException("overlayFrameNumber", overlayFrameNumber, "Must be a positive, non-zero number.");

				byte[] data;
				int key = ((overlayFrameNumber - 1) << 8) | ((overlayGroupNumber - 1) & 0x000000ff);

				if (!_overlayData.ContainsKey(key))
				{
					lock (this.Parent.SyncLock)
					{
						if (!_overlayData.ContainsKey(key))
						{
							_overlayData.Add(key, new WeakReference(null));
						}
					}
				}

				try
				{
					data = _overlayData[key].Target as byte[];
				}
				catch (InvalidOperationException)
				{
					_overlayData[key] = new WeakReference(data = null);
				}

				if (!_overlayData[key].IsAlive || data == null)
				{
					lock (this.Parent.SyncLock)
					{
						_overlayData[key].Target = data = CreateNormalizedOverlayData(overlayGroupNumber, overlayFrameNumber);
					}
				}

				return data;
			}

			protected abstract byte[] CreateNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber);

			public sealed override void Unload()
			{
				this.OnUnloading();
				_pixelData = new WeakReference(null);
				_overlayData.Clear();
				this.OnUnloaded();
			}

			protected virtual void OnUnloading() {}
			protected virtual void OnUnloaded() {}
		}

		#region Deprecated Members

		[Obsolete("This formerly abstract method is no longer called.")]
		protected virtual byte[] CreateFrameNormalizedPixelData(int frameNumber)
		{
			return null;
		}

		[Obsolete("This formerly abstract method is no longer called.")]
		protected sealed override void OnGetFrameNormalizedPixelData(int frameNumber, out byte[] pixelData)
		{
			base.OnGetFrameNormalizedPixelData(frameNumber, out pixelData);
		}

		[Obsolete("This formerly abstract method is no longer called.")]
		protected virtual byte[] CreateFrameNormalizedOverlayData(int overlayNumber, int frameNumber)
		{
			return null;
		}

		[Obsolete("This formerly abstract method is no longer called.")]
		protected override sealed void OnGetFrameNormalizedOverlayData(int overlayNumber, int frameNumber, out byte[] overlayData)
		{
			base.OnGetFrameNormalizedOverlayData(overlayNumber, frameNumber, out overlayData);
		}

		[Obsolete("This virtual method is no longer called.")]
		protected virtual void OnUnloadFrameData(int frameNumber) {}

		[Obsolete("Use ISopDataSource.GetFrameData(frameNumber).Unload() instead.")]
		public sealed override void UnloadFrameData(int frameNumber)
		{
			base.UnloadFrameData(frameNumber);
		}

		#endregion
	}
}
