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
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class StandardSopDataSource : SopDataSource
	{
		//I hate doing this, but it's horribly inefficient for all subclasses to do their own locking.
		protected readonly object SyncLock = new object();
		private volatile ISopFrameData[] _frameData;

		protected StandardSopDataSource() : base() {}

		protected abstract StandardSopFrameData CreateFrameData(int frameNumber);

		public override ISopFrameData GetFrameData(int frameNumber)
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

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				lock(SyncLock)
				{
					if (_frameData != null)
					{
						foreach (ISopFrameData frameData in _frameData)
						{
							frameData.Dispose();
						}
					}
				}
			}
		}

		#region StandardSopFrameData class

		protected abstract class StandardSopFrameData : SopFrameData
		{
			private readonly Dictionary<int, byte[]> _overlayData = new Dictionary<int, byte[]>();
			private byte[] _pixelData = null;

			public StandardSopFrameData(int frameNumber, StandardSopDataSource parent) : base(frameNumber, parent) {}

			public new StandardSopDataSource Parent
			{
				get { return (StandardSopDataSource) base.Parent; }
			}

			public override byte[] GetNormalizedPixelData()
			{
				lock (this.Parent.SyncLock)
				{
					if (_pixelData == null)
					{
						_pixelData = CreateNormalizedPixelData();
						if (_pixelData != null)
							Diagnostics.OnLargeObjectAllocated(_pixelData.Length);
					}

					return _pixelData;
				}
			}

			protected abstract byte[] CreateNormalizedPixelData();

			public override byte[] GetNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
			{
				if(overlayGroupNumber < 1)
					throw new ArgumentOutOfRangeException("overlayGroupNumber", overlayGroupNumber, "Must be a positive, non-zero number.");
				if (overlayFrameNumber < 1)
					throw new ArgumentOutOfRangeException("overlayFrameNumber", overlayFrameNumber, "Must be a positive, non-zero number.");

				int key = ((overlayFrameNumber - 1) << 8) | ((overlayGroupNumber - 1) & 0x000000ff);

				lock (this.Parent.SyncLock)
				{
					byte[] data;
					if (!_overlayData.TryGetValue(key, out data) || data == null)
					{
						_overlayData[key] = data = CreateNormalizedOverlayData(overlayGroupNumber, overlayFrameNumber);
						if (data != null)
							Diagnostics.OnLargeObjectAllocated(data.Length);
					}

					return data;
				}
			}

			protected abstract byte[] CreateNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber);

			public override sealed void Unload()
			{
				lock (this.Parent.SyncLock)
				{
					ReportLargeObjectsUnloaded();

					this.OnUnloading();
					_pixelData = null;
					_overlayData.Clear();
					this.OnUnloaded();
				}
			}

			protected virtual void OnUnloading()
			{
				
			}
			
			protected virtual void OnUnloaded()
			{
				
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);

				if (disposing)
				{
					lock (Parent.SyncLock)
					{
						ReportLargeObjectsUnloaded();
					}
				}
			}

			private void ReportLargeObjectsUnloaded()
			{
				if (_pixelData != null)
					Diagnostics.OnLargeObjectReleased(_pixelData.Length);

				foreach (byte[] overlayData in _overlayData.Values)
				{
					if (overlayData != null)
						Diagnostics.OnLargeObjectReleased(overlayData.Length);
				}
			}
		}

		#endregion
	}
}
